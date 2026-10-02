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
    [Authorize(Roles = "Admin")]
    public class GeneralSettingsController : ControllerBase
    {
        private readonly IGeneralSettingsService _generalSettingsService;

        public GeneralSettingsController(IGeneralSettingsService generalSettingsService)
        {
            _generalSettingsService = generalSettingsService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<GeneralSettingsViewModel>>> GetAll()
        {
            var settings = await _generalSettingsService.GetAllAsync();
            return Ok(settings);
        }

        [HttpGet("active")]
        public async Task<ActionResult<GeneralSettingsViewModel>> GetActive()
        {
            var settings = await _generalSettingsService.GetActiveAsync();
            if (settings == null)
            {
                return NotFound();
            }

            return Ok(settings);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GeneralSettingsViewModel>> GetById(int id)
        {
            var settings = await _generalSettingsService.GetByIdAsync(id);
            if (settings == null)
            {
                return NotFound();
            }

            return Ok(settings);
        }

        [HttpPost]
        public async Task<ActionResult<GeneralSettingsViewModel>> Create(CreateGeneralSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _generalSettingsService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.CompanyId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<GeneralSettingsViewModel>> Update(int id, UpdateGeneralSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _generalSettingsService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "GeneralSettings not found.")
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
            var deleted = await _generalSettingsService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
