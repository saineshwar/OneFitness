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
    public class InstallmentController : ControllerBase
    {
        private readonly IInstallmentService _installmentService;

        public InstallmentController(IInstallmentService installmentService)
        {
            _installmentService = installmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<InstallmentViewModel>>> GetAll()
        {
            var installments = await _installmentService.GetAllAsync();
            return Ok(installments);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<InstallmentViewModel>> GetById(int id)
        {
            var installment = await _installmentService.GetByIdAsync(id);
            if (installment == null)
            {
                return NotFound();
            }

            return Ok(installment);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<InstallmentViewModel>> Create(CreateInstallmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _installmentService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.InstallmentId }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<InstallmentViewModel>> Update(int id, UpdateInstallmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _installmentService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "Installment not found.")
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
            var deleted = await _installmentService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
