using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOutController : ControllerBase
    {
        private readonly IWorkOutService _workOutService;

        public WorkOutController(IWorkOutService workOutService)
        {
            _workOutService = workOutService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<WorkOutViewModel>>> GetAll()
        {
            var workOuts = await _workOutService.GetAllAsync();
            return Ok(workOuts);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<WorkOutViewModel>> GetById(int id)
        {
            var workOut = await _workOutService.GetByIdAsync(id);
            if (workOut == null)
            {
                return NotFound();
            }

            return Ok(workOut);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<WorkOutViewModel>> Create(CreateWorkOutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _workOutService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.WorkOutId }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<WorkOutViewModel>> Update(int id, UpdateWorkOutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _workOutService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "WorkOut not found.")
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _workOutService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
