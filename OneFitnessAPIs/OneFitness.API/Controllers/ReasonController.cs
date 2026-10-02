using Microsoft.AspNetCore.Mvc;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReasonController : ControllerBase
    {
        private readonly IReasonService _reasonService;

        public ReasonController(IReasonService reasonService)
        {
            _reasonService = reasonService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ReasonViewModel>>> GetAll()
        {
            var reasons = await _reasonService.GetAllAsync();
            return Ok(reasons);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReasonViewModel>> GetById(int id)
        {
            var reason = await _reasonService.GetByIdAsync(id);
            if (reason == null)
            {
                return NotFound();
            }

            return Ok(reason);
        }

        [HttpPost]
        public async Task<ActionResult<ReasonViewModel>> Create(CreateReasonViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _reasonService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ReasonId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ReasonViewModel>> Update(int id, UpdateReasonViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _reasonService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "Reason not found.")
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _reasonService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
