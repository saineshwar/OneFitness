using Microsoft.EntityFrameworkCore;
using OneFitness.Entities;
using OneFitness.Repository.EFContext;
using OneFitness.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Repository.Implementation
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Member>> GetAllAsync()
        {
            return await _context.Members.AsNoTracking().OrderByDescending(m => m.MemberId).ToListAsync();
        }

        public async Task<(IReadOnlyList<Member> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? search)
        {
            var query = _context.Members.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(m =>
                    EF.Functions.Like(m.FirstName, $"%{term}%") ||
                    (m.LastName != null && EF.Functions.Like(m.LastName, $"%{term}%")) ||
                    EF.Functions.Like(m.MemberNo, $"%{term}%") ||
                    (m.MobileNo != null && EF.Functions.Like(m.MobileNo, $"%{term}%")) ||
                    (m.EmailId != null && EF.Functions.Like(m.EmailId, $"%{term}%")));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.MemberId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Member?> GetByIdAsync(long memberId)
        {
            return await _context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.MemberId == memberId);
        }

        public async Task<Member?> GetByMemberNoAsync(string memberNo)
        {
            return await _context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.MemberNo == memberNo);
        }

        public async Task<Member> AddAsync(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> UpdateAsync(Member member)
        {
            var existing = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == member.MemberId);
            if (existing == null)
            {
                return false;
            }

            _context.Entry(existing).CurrentValues.SetValues(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long memberId)
        {
            var existing = await _context.Members.FirstOrDefaultAsync(m => m.MemberId == memberId);
            if (existing == null)
            {
                return false;
            }

            _context.Members.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MobileNoExistsAsync(string? mobileNo, long? excludeMemberId = null)
        {
            if (string.IsNullOrWhiteSpace(mobileNo))
            {
                return false;
            }

            return await _context.Members.AnyAsync(m => m.MobileNo == mobileNo && m.MemberId != excludeMemberId);
        }

        public async Task<bool> EmailExistsAsync(string? emailId, long? excludeMemberId = null)
        {
            if (string.IsNullOrWhiteSpace(emailId))
            {
                return false;
            }

            return await _context.Members.AnyAsync(m => m.EmailId == emailId && m.MemberId != excludeMemberId);
        }

        public async Task<MemberPhoto?> GetPhotoAsync(long memberId)
        {
            return await _context.MemberPhotos.AsNoTracking().FirstOrDefaultAsync(p => p.MemberId == memberId);
        }

        public async Task SavePhotoAsync(MemberPhoto photo)
        {
            var existing = await _context.MemberPhotos.FirstOrDefaultAsync(p => p.MemberId == photo.MemberId);
            if (existing == null)
            {
                _context.MemberPhotos.Add(photo);
            }
            else
            {
                existing.PhotoData = photo.PhotoData;
                existing.ContentType = photo.ContentType;
                existing.ModifiedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeletePhotoAsync(long memberId)
        {
            var existing = await _context.MemberPhotos.FirstOrDefaultAsync(p => p.MemberId == memberId);
            if (existing == null)
            {
                return false;
            }

            _context.MemberPhotos.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
