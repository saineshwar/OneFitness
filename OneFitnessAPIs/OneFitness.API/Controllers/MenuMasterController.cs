using Microsoft.AspNetCore.Mvc;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuMasterController : ControllerBase
    {
        private readonly IMenuMasterService _menuMasterService;

        public MenuMasterController(IMenuMasterService menuMasterService)
        {
            _menuMasterService = menuMasterService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MenuMasterViewModel>>> GetAll()
        {
            var menuMasters = await _menuMasterService.GetAllAsync();
            return Ok(menuMasters);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MenuMasterViewModel>> GetById(int id)
        {
            var menuMaster = await _menuMasterService.GetByIdAsync(id);
            if (menuMaster == null)
            {
                return NotFound();
            }

            return Ok(menuMaster);
        }

        [HttpPost]
        public async Task<ActionResult<MenuMasterViewModel>> Create(CreateMenuMasterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _menuMasterService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.MenuId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MenuMasterViewModel>> Update(int id, UpdateMenuMasterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _menuMasterService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "Menu not found.")
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
            var deleted = await _menuMasterService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
