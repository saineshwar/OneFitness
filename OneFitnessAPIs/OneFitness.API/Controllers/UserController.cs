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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserViewModel>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserViewModel>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserViewModel>> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _userService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.UserId }, result.Data);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<UserViewModel>> Update(int id, UpdateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _userService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "User not found.")
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return Ok(result.Data);
        }

        [HttpPost("{id:int}/reset-password")]
        public async Task<ActionResult<UserViewModel>> ResetPassword(int id, ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _userService.ResetPasswordAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "User not found.")
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
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
