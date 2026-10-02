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
    public class MembershipTypeController : ControllerBase
    {
        private readonly IMembershipTypeService _membershipTypeService;

        public MembershipTypeController(IMembershipTypeService membershipTypeService)
        {
            _membershipTypeService = membershipTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MembershipTypeViewModel>>> GetAll()
        {
            var membershipTypes = await _membershipTypeService.GetAllAsync();
            return Ok(membershipTypes);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MembershipTypeViewModel>> GetById(int id)
        {
            var membershipType = await _membershipTypeService.GetByIdAsync(id);
            if (membershipType == null)
            {
                return NotFound();
            }

            return Ok(membershipType);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<MembershipTypeViewModel>> Create(CreateMembershipTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _membershipTypeService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.MembershipTypeId }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MembershipTypeViewModel>> Update(int id, UpdateMembershipTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _membershipTypeService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "MembershipType not found.")
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
            var deleted = await _membershipTypeService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
