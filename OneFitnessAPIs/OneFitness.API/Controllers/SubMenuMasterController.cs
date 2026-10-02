using Microsoft.AspNetCore.Mvc;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubMenuMasterController : ControllerBase
    {
        private readonly ISubMenuMasterService _subMenuMasterService;

        public SubMenuMasterController(ISubMenuMasterService subMenuMasterService)
        {
            _subMenuMasterService = subMenuMasterService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<SubMenuMasterViewModel>>> GetAll()
        {
            var subMenuMasters = await _subMenuMasterService.GetAllAsync();
            return Ok(subMenuMasters);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubMenuMasterViewModel>> GetById(int id)
        {
            var subMenuMaster = await _subMenuMasterService.GetByIdAsync(id);
            if (subMenuMaster == null)
            {
                return NotFound();
            }

            return Ok(subMenuMaster);
        }

        [HttpPost]
        public async Task<ActionResult<SubMenuMasterViewModel>> Create(CreateSubMenuMasterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _subMenuMasterService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.SubMenuId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SubMenuMasterViewModel>> Update(int id, UpdateSubMenuMasterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _subMenuMasterService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "SubMenu not found.")
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
            var deleted = await _subMenuMasterService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
