using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await _context.Users.AsNoTracking().OrderBy(u => u.UserId).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int userId)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (existing == null)
            {
                return false;
            }

            _context.Users.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserNameExistsAsync(string userName, int? excludeUserId = null)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName && u.UserId != excludeUserId);
        }

        public async Task<bool> EmailExistsAsync(string emailId, int? excludeUserId = null)
        {
            return await _context.Users.AnyAsync(u => u.EmailId == emailId && u.UserId != excludeUserId);
        }
    }
}
