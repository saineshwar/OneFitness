using OneFitness.Entities;
using OneFitness.Repository.Interfaces;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneFitness.Services.Implementation
{
    public class ReasonService : IReasonService
    {
        private readonly IReasonRepository _reasonRepository;

        public ReasonService(IReasonRepository reasonRepository)
        {
            _reasonRepository = reasonRepository;
        }

        public async Task<IReadOnlyList<ReasonViewModel>> GetAllAsync()
        {
            var reasons = await _reasonRepository.GetAllAsync();
            return reasons.Select(ToViewModel).ToList();
        }

        public async Task<ReasonViewModel?> GetByIdAsync(int reasonId)
        {
            var reason = await _reasonRepository.GetByIdAsync(reasonId);
            return reason == null ? null : ToViewModel(reason);
        }

        public async Task<ReasonServiceResult<ReasonViewModel>> CreateAsync(CreateReasonViewModel model)
        {
            if (await _reasonRepository.ReasonNameExistsAsync(model.ReasonName))
            {
                return ReasonServiceResult<ReasonViewModel>.Failure("Reason already exists.");
            }

            var reason = new Reason
            {
                ReasonName = model.ReasonName
            };

            var created = await _reasonRepository.AddAsync(reason);
            return ReasonServiceResult<ReasonViewModel>.Success(ToViewModel(created));
        }

        public async Task<ReasonServiceResult<ReasonViewModel>> UpdateAsync(int reasonId, UpdateReasonViewModel model)
        {
            var existing = await _reasonRepository.GetByIdAsync(reasonId);
            if (existing == null)
            {
                return ReasonServiceResult<ReasonViewModel>.Failure("Reason not found.");
            }

            if (await _reasonRepository.ReasonNameExistsAsync(model.ReasonName, reasonId))
            {
                return ReasonServiceResult<ReasonViewModel>.Failure("Reason already exists.");
            }

            existing.ReasonName = model.ReasonName;

            await _reasonRepository.UpdateAsync(existing);
            return ReasonServiceResult<ReasonViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int reasonId)
        {
            return _reasonRepository.DeleteAsync(reasonId);
        }

        private static ReasonViewModel ToViewModel(Reason reason)
        {
            return new ReasonViewModel
            {
                ReasonId = reason.ReasonId,
                ReasonName = reason.ReasonName
            };
        }
    }
}
