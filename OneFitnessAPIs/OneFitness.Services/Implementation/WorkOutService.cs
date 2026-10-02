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
    public class WorkOutService : IWorkOutService
    {
        private readonly IWorkOutRepository _workOutRepository;

        public WorkOutService(IWorkOutRepository workOutRepository)
        {
            _workOutRepository = workOutRepository;
        }

        public async Task<IReadOnlyList<WorkOutViewModel>> GetAllAsync()
        {
            var workOuts = await _workOutRepository.GetAllAsync();
            return workOuts.Select(ToViewModel).ToList();
        }

        public async Task<WorkOutViewModel?> GetByIdAsync(int workOutId)
        {
            var workOut = await _workOutRepository.GetByIdAsync(workOutId);
            return workOut == null ? null : ToViewModel(workOut);
        }

        public async Task<WorkOutServiceResult<WorkOutViewModel>> CreateAsync(CreateWorkOutViewModel model)
        {
            if (await _workOutRepository.WorkOutNameExistsAsync(model.WorkOutName))
            {
                return WorkOutServiceResult<WorkOutViewModel>.Failure("WorkOut already exists.");
            }

            var workOut = new WorkOut
            {
                WorkOutName = model.WorkOutName,
                Description = model.Description,
                Status = model.Status,
                CreatedOn = DateTime.UtcNow
            };

            var created = await _workOutRepository.AddAsync(workOut);
            return WorkOutServiceResult<WorkOutViewModel>.Success(ToViewModel(created));
        }

        public async Task<WorkOutServiceResult<WorkOutViewModel>> UpdateAsync(int workOutId, UpdateWorkOutViewModel model)
        {
            var existing = await _workOutRepository.GetByIdAsync(workOutId);
            if (existing == null)
            {
                return WorkOutServiceResult<WorkOutViewModel>.Failure("WorkOut not found.");
            }

            if (await _workOutRepository.WorkOutNameExistsAsync(model.WorkOutName, workOutId))
            {
                return WorkOutServiceResult<WorkOutViewModel>.Failure("WorkOut already exists.");
            }

            existing.WorkOutName = model.WorkOutName;
            existing.Description = model.Description;
            existing.Status = model.Status;
            existing.ModifiedOn = DateTime.UtcNow;

            await _workOutRepository.UpdateAsync(existing);
            return WorkOutServiceResult<WorkOutViewModel>.Success(ToViewModel(existing));
        }

        public Task<bool> DeleteAsync(int workOutId)
        {
            return _workOutRepository.DeleteAsync(workOutId);
        }

        private static WorkOutViewModel ToViewModel(WorkOut workOut)
        {
            return new WorkOutViewModel
            {
                WorkOutId = workOut.WorkOutId,
                WorkOutName = workOut.WorkOutName,
                Description = workOut.Description,
                Status = workOut.Status,
                CreatedOn = workOut.CreatedOn,
                ModifiedOn = workOut.ModifiedOn
            };
        }
    }
}
