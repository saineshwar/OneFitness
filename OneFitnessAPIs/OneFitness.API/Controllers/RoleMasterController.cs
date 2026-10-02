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
    public class RoleMasterController : ControllerBase
    {
        private readonly IRoleMasterService _roleMasterService;

        public RoleMasterController(IRoleMasterService roleMasterService)
        {
            _roleMasterService = roleMasterService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<RoleMasterViewModel>>> GetAll()
        {
            var roles = await _roleMasterService.GetAllAsync();
            return Ok(roles);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IReadOnlyList<RoleMasterViewModel>>> GetAllActive([FromQuery] int? excludeRoleId)
        {
            var roles = await _roleMasterService.GetAllActiveAsync(excludeRoleId);
            return Ok(roles);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoleMasterViewModel>> GetById(int id)
        {
            var role = await _roleMasterService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<RoleMasterViewModel>> Create(CreateRoleMasterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _roleMasterService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.RoleId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<RoleMasterViewModel>> Update(int id, UpdateRoleMasterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _roleMasterService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "Role not found.")
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
            var deleted = await _roleMasterService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
