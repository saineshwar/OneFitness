using OneFitness.Entities;
using OneFitness.Repository.Interfaces;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Services.Implementation
{
    public class MemberService : IMemberService
    {
        private const string UniqueKeyChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int MaxPhotoBytes = 1024 * 1024;
        private static readonly HashSet<string> AllowedPhotoContentTypes = new HashSet<string> { "image/jpeg", "image/png", "image/webp" };

        private readonly IMemberRepository _memberRepository;
        private readonly Random _random = new Random();

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<IReadOnlyList<MemberViewModel>> GetAllAsync()
        {
            var members = await _memberRepository.GetAllAsync();
            return members.Select(ToViewModel).ToList();
        }

        public async Task<PagedResultViewModel<MemberViewModel>> GetPagedAsync(int page, int pageSize, string? search)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var (items, totalCount) = await _memberRepository.GetPagedAsync(page, pageSize, search);
            return new PagedResultViewModel<MemberViewModel>
            {
                Items = items.Select(ToViewModel).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<MemberViewModel?> GetByIdAsync(long memberId)
        {
            var member = await _memberRepository.GetByIdAsync(memberId);
            return member == null ? null : ToViewModel(member);
        }

        public async Task<MemberServiceResult<MemberViewModel>> CreateAsync(CreateMemberViewModel model)
        {
            if (await _memberRepository.MobileNoExistsAsync(model.MobileNo))
            {
                return MemberServiceResult<MemberViewModel>.Failure("MobileNo already exists.");
            }

            if (await _memberRepository.EmailExistsAsync(model.EmailId))
            {
                return MemberServiceResult<MemberViewModel>.Failure("EmailId already exists.");
            }

            var member = new Member
            {
                MemberNo = GenerateMemberNo(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                MiddleName = model.MiddleName,
                DOB = model.DOB,
                Age = model.Age,
                MobileNo = model.MobileNo,
                EmailId = model.EmailId,
                GenderId = model.GenderId,
                Address = model.Address,
                JoiningDate = model.JoiningDate!.Value,
                EmergencyContactName = model.EmergencyContactName,
                EmergencyContactNo = model.EmergencyContactNo,
                Status = model.Status,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _memberRepository.AddAsync(member);
            return MemberServiceResult<MemberViewModel>.Success(ToViewModel(created));
        }

        public async Task<MemberServiceResult<MemberViewModel>> UpdateAsync(long memberId, UpdateMemberViewModel model)
        {
            var existing = await _memberRepository.GetByIdAsync(memberId);
            if (existing == null)
            {
                return MemberServiceResult<MemberViewModel>.Failure("Member not found.");
            }

            if (await _memberRepository.MobileNoExistsAsync(model.MobileNo, memberId))
            {
                return MemberServiceResult<MemberViewModel>.Failure("MobileNo already exists.");
            }

            if (await _memberRepository.EmailExistsAsync(model.EmailId, memberId))
            {
                return MemberServiceResult<MemberViewModel>.Failure("EmailId already exists.");
            }

            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.MiddleName = model.MiddleName;
            existing.DOB = model.DOB;
            existing.Age = model.Age;
            existing.MobileNo = model.MobileNo;
            existing.EmailId = model.EmailId;
            existing.GenderId = model.GenderId;
            existing.Address = model.Address;
            existing.JoiningDate = model.JoiningDate!.Value;
            existing.EmergencyContactName = model.EmergencyContactName;
            existing.EmergencyContactNo = model.EmergencyContactNo;
            existing.Status = model.Status;
            existing.ModifiedOn = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(existing);
            return MemberServiceResult<MemberViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(long memberId)
        {
            return _memberRepository.DeleteAsync(memberId);
        }

        public async Task<bool> ActivateAsync(long memberId)
        {
            var existing = await _memberRepository.GetByIdAsync(memberId);
            if (existing == null)
            {
                return false;
            }

            existing.Status = true;
            existing.ModifiedOn = DateTime.UtcNow;
            return await _memberRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeactivateAsync(long memberId)
        {
            var existing = await _memberRepository.GetByIdAsync(memberId);
            if (existing == null)
            {
                return false;
            }

            existing.Status = false;
            existing.ModifiedOn = DateTime.UtcNow;
            return await _memberRepository.UpdateAsync(existing);
        }

        public async Task<MemberPhotoViewModel?> GetPhotoAsync(long memberId)
        {
            var photo = await _memberRepository.GetPhotoAsync(memberId);
            if (photo == null)
            {
                return null;
            }

            return new MemberPhotoViewModel
            {
                Photo = $"data:{photo.ContentType};base64,{Convert.ToBase64String(photo.PhotoData)}"
            };
        }

        public async Task<MemberServiceResult<MemberPhotoViewModel>> SavePhotoAsync(long memberId, MemberPhotoViewModel model)
        {
            if (await _memberRepository.GetByIdAsync(memberId) == null)
            {
                return MemberServiceResult<MemberPhotoViewModel>.Failure("Member not found.");
            }

            // Expected format: data:<content-type>;base64,<payload>
            const string base64Marker = ";base64,";
            var markerIndex = model.Photo.IndexOf(base64Marker, StringComparison.Ordinal);
            if (!model.Photo.StartsWith("data:", StringComparison.Ordinal) || markerIndex < 0)
            {
                return MemberServiceResult<MemberPhotoViewModel>.Failure("Photo must be a base64 data URL.");
            }

            var contentType = model.Photo.Substring(5, markerIndex - 5).ToLowerInvariant();
            if (!AllowedPhotoContentTypes.Contains(contentType))
            {
                return MemberServiceResult<MemberPhotoViewModel>.Failure("Photo must be a JPEG, PNG or WebP image.");
            }

            byte[] photoData;
            try
            {
                photoData = Convert.FromBase64String(model.Photo.Substring(markerIndex + base64Marker.Length));
            }
            catch (FormatException)
            {
                return MemberServiceResult<MemberPhotoViewModel>.Failure("Photo data is not valid base64.");
            }

            if (photoData.Length == 0 || photoData.Length > MaxPhotoBytes)
            {
                return MemberServiceResult<MemberPhotoViewModel>.Failure("Photo must be smaller than 1 MB.");
            }

            await _memberRepository.SavePhotoAsync(new MemberPhoto
            {
                MemberId = memberId,
                PhotoData = photoData,
                ContentType = contentType,
                CreatedOn = DateTime.UtcNow
            });

            return MemberServiceResult<MemberPhotoViewModel>.Success(model);
        }

        public Task<bool> DeletePhotoAsync(long memberId)
        {
            return _memberRepository.DeletePhotoAsync(memberId);
        }

        private string GenerateMemberNo()
        {
            var dayOfYear = DateTime.UtcNow.DayOfYear;
            var uniqueKey = new string(Enumerable.Range(0, 10).Select(_ => UniqueKeyChars[_random.Next(UniqueKeyChars.Length)]).ToArray());
            return $"OFV{dayOfYear}{uniqueKey}";
        }

        private static MemberViewModel ToViewModel(Member member)
        {
            return new MemberViewModel
            {
                MemberId = member.MemberId,
                MemberNo = member.MemberNo,
                FirstName = member.FirstName,
                LastName = member.LastName,
                MiddleName = member.MiddleName,
                DOB = member.DOB,
                Age = member.Age,
                MobileNo = member.MobileNo,
                EmailId = member.EmailId,
                GenderId = member.GenderId,
                Address = member.Address,
                JoiningDate = member.JoiningDate,
                EmergencyContactName = member.EmergencyContactName,
                EmergencyContactNo = member.EmergencyContactNo,
                Status = member.Status,
                CreatedOn = member.CreatedOn,
                ModifiedOn = member.ModifiedOn
            };
        }
    }
}
