using Microsoft.AspNetCore.Mvc;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuCategoryController : ControllerBase
    {
        private readonly IMenuCategoryService _menuCategoryService;

        public MenuCategoryController(IMenuCategoryService menuCategoryService)
        {
            _menuCategoryService = menuCategoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MenuCategoryViewModel>>> GetAll()
        {
            var menuCategories = await _menuCategoryService.GetAllAsync();
            return Ok(menuCategories);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MenuCategoryViewModel>> GetById(int id)
        {
            var menuCategory = await _menuCategoryService.GetByIdAsync(id);
            if (menuCategory == null)
            {
                return NotFound();
            }

            return Ok(menuCategory);
        }

        [HttpPost]
        public async Task<ActionResult<MenuCategoryViewModel>> Create(CreateMenuCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _menuCategoryService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.MenuCategoryId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<MenuCategoryViewModel>> Update(int id, UpdateMenuCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _menuCategoryService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "MenuCategory not found.")
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
            var deleted = await _menuCategoryService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
