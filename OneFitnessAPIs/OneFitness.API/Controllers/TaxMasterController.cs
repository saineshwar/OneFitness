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
    public class TaxMasterController : ControllerBase
    {
        private readonly ITaxMasterService _taxMasterService;

        public TaxMasterController(ITaxMasterService taxMasterService)
        {
            _taxMasterService = taxMasterService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TaxMasterViewModel>>> GetAll()
        {
            var taxMasters = await _taxMasterService.GetAllAsync();
            return Ok(taxMasters);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaxMasterViewModel>> GetById(int id)
        {
            var taxMaster = await _taxMasterService.GetByIdAsync(id);
            if (taxMaster == null)
            {
                return NotFound();
            }

            return Ok(taxMaster);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<TaxMasterViewModel>> Create(CreateTaxMasterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _taxMasterService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.TaxId }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<TaxMasterViewModel>> Update(int id, UpdateTaxMasterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _taxMasterService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "TaxMaster not found.")
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
            var deleted = await _taxMasterService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
