using Microsoft.AspNetCore.Mvc;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnquiryController : ControllerBase
    {
        private readonly IEnquiryService _enquiryService;

        public EnquiryController(IEnquiryService enquiryService)
        {
            _enquiryService = enquiryService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<EnquiryViewModel>>> GetAll()
        {
            var enquiries = await _enquiryService.GetAllAsync();
            return Ok(enquiries);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EnquiryViewModel>> GetById(int id)
        {
            var enquiry = await _enquiryService.GetByIdAsync(id);
            if (enquiry == null)
            {
                return NotFound();
            }

            return Ok(enquiry);
        }

        [HttpPost]
        public async Task<ActionResult<EnquiryViewModel>> Create(CreateEnquiryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _enquiryService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.EnquiryId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<EnquiryViewModel>> Update(int id, UpdateEnquiryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _enquiryService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "Enquiry not found.")
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
            var deleted = await _enquiryService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
