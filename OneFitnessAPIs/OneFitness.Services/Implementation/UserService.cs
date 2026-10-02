using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OneFitness.Entities;
using OneFitness.Repository.Interfaces;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OneFitness.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAssignedRoleRepository _assignedRoleRepository;
        private readonly IRoleMasterRepository _roleMasterRepository;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly JwtOptions _jwtOptions;

        public UserService(
            IUserRepository userRepository,
            IAssignedRoleRepository assignedRoleRepository,
            IRoleMasterRepository roleMasterRepository,
            IOptions<JwtOptions> jwtOptions)
        {
            _userRepository = userRepository;
            _assignedRoleRepository = assignedRoleRepository;
            _roleMasterRepository = roleMasterRepository;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<IReadOnlyList<UserViewModel>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var assignedRoles = await _assignedRoleRepository.GetAllAsync();
            var roles = await _roleMasterRepository.GetAllAsync();

            var roleIdByUserId = assignedRoles.ToDictionary(a => a.UserId, a => a.RoleId);
            var roleNameByRoleId = roles.ToDictionary(r => r.RoleId, r => r.RoleName);

            return users
                .Select(u =>
                {
                    roleIdByUserId.TryGetValue(u.UserId, out var roleId);
                    roleNameByRoleId.TryGetValue(roleId, out var roleName);
                    return ToViewModel(u, roleIdByUserId.ContainsKey(u.UserId) ? roleId : (int?)null, roleName);
                })
                .ToList();
        }

        public async Task<UserViewModel?> GetByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var (roleId, roleName) = await ResolveRoleAsync(userId);
            return ToViewModel(user, roleId, roleName);
        }

        public async Task<UserServiceResult<UserViewModel>> CreateAsync(CreateUserViewModel model)
        {
            if (await _userRepository.UserNameExistsAsync(model.UserName))
            {
                return UserServiceResult<UserViewModel>.Failure("Username already exists.");
            }

            if (await _userRepository.EmailExistsAsync(model.EmailId))
            {
                return UserServiceResult<UserViewModel>.Failure("EmailId already exists.");
            }

            var role = await _roleMasterRepository.GetByIdAsync(model.RoleId!.Value);
            if (role == null)
            {
                return UserServiceResult<UserViewModel>.Failure("Selected role does not exist.");
            }

            var user = new User
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailId = model.EmailId,
                MobileNo = model.MobileNo,
                Gender = model.Gender,
                Status = true,
                IsFirstLogin = true,
                CreatedOn = DateTime.UtcNow
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

            var created = await _userRepository.AddAsync(user);

            await _assignedRoleRepository.AddAsync(new AssignedRole
            {
                UserId = created.UserId,
                RoleId = role.RoleId,
                Status = true,
                CreatedOn = DateTime.UtcNow
            });

            return UserServiceResult<UserViewModel>.Success(ToViewModel(created, role.RoleId, role.RoleName));
        }

        public async Task<UserServiceResult<UserViewModel>> UpdateAsync(int userId, UpdateUserViewModel model)
        {
            var existing = await _userRepository.GetByIdAsync(userId);
            if (existing == null)
            {
                return UserServiceResult<UserViewModel>.Failure("User not found.");
            }

            if (await _userRepository.EmailExistsAsync(model.EmailId, userId))
            {
                return UserServiceResult<UserViewModel>.Failure("EmailId already exists.");
            }

            var role = await _roleMasterRepository.GetByIdAsync(model.RoleId!.Value);
            if (role == null)
            {
                return UserServiceResult<UserViewModel>.Failure("Selected role does not exist.");
            }

            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.EmailId = model.EmailId;
            existing.MobileNo = model.MobileNo;
            existing.Gender = model.Gender;
            existing.Status = model.Status;
            existing.ModifiedOn = DateTime.UtcNow;

            await _userRepository.UpdateAsync(existing);

            var assignedRole = await _assignedRoleRepository.GetByUserIdAsync(userId);
            if (assignedRole == null)
            {
                await _assignedRoleRepository.AddAsync(new AssignedRole
                {
                    UserId = userId,
                    RoleId = role.RoleId,
                    Status = true,
                    CreatedOn = DateTime.UtcNow
                });
            }
            else
            {
                assignedRole.RoleId = role.RoleId;
                assignedRole.ModifiedOn = DateTime.UtcNow;
                await _assignedRoleRepository.UpdateAsync(assignedRole);
            }

            return UserServiceResult<UserViewModel>.Success(ToViewModel(existing, role.RoleId, role.RoleName));
        }

        public async Task<UserServiceResult<UserViewModel>> ResetPasswordAsync(int userId, ResetPasswordViewModel model)
        {
            var existing = await _userRepository.GetByIdAsync(userId);
            if (existing == null)
            {
                return UserServiceResult<UserViewModel>.Failure("User not found.");
            }

            existing.PasswordHash = _passwordHasher.HashPassword(existing, model.NewPassword);
            existing.ModifiedOn = DateTime.UtcNow;
            await _userRepository.UpdateAsync(existing);

            var (roleId, roleName) = await ResolveRoleAsync(userId);
            return UserServiceResult<UserViewModel>.Success(ToViewModel(existing, roleId, roleName));
        }

        public Task<bool> DeleteAsync(int userId)
        {
            return _userRepository.DeleteAsync(userId);
        }

        public async Task<UserServiceResult<LoginResultViewModel>> LoginAsync(LoginViewModel model)
        {
            var user = await _userRepository.GetByUserNameAsync(model.UserName);
            if (user == null)
            {
                return UserServiceResult<LoginResultViewModel>.Failure("Invalid username or password.");
            }

            if (!user.Status)
            {
                return UserServiceResult<LoginResultViewModel>.Failure("This account is inactive.");
            }

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verification == PasswordVerificationResult.Failed)
            {
                return UserServiceResult<LoginResultViewModel>.Failure("Invalid username or password.");
            }

            var (roleId, roleName) = await ResolveRoleAsync(user.UserId);
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, user.FirstName),
            };
            if (!string.IsNullOrEmpty(roleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return UserServiceResult<LoginResultViewModel>.Success(new LoginResultViewModel
            {
                Token = tokenString,
                ExpiresAtUtc = expiresAtUtc,
                UserId = user.UserId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = roleId,
                RoleName = roleName
            });
        }

        private async Task<(int? RoleId, string? RoleName)> ResolveRoleAsync(int userId)
        {
            var assignedRole = await _assignedRoleRepository.GetByUserIdAsync(userId);
            if (assignedRole == null)
            {
                return (null, null);
            }

            var role = await _roleMasterRepository.GetByIdAsync(assignedRole.RoleId);
            return (assignedRole.RoleId, role?.RoleName);
        }

        private static UserViewModel ToViewModel(User user, int? roleId, string? roleName)
        {
            return new UserViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailId = user.EmailId,
                MobileNo = user.MobileNo,
                Gender = user.Gender,
                RoleId = roleId,
                RoleName = roleName,
                Status = user.Status,
                IsFirstLogin = user.IsFirstLogin,
                CreatedOn = user.CreatedOn,
                ModifiedOn = user.ModifiedOn
            };
        }
    }
}
