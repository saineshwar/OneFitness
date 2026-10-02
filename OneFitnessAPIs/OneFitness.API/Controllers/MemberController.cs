using Microsoft.AspNetCore.Mvc;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MemberViewModel>>> GetAll()
        {
            var members = await _memberService.GetAllAsync();
            return Ok(members);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultViewModel<MemberViewModel>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _memberService.GetPagedAsync(page, pageSize, search);
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<MemberViewModel>> GetById(long id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null)
            {
                return NotFound();
            }

            return Ok(member);
        }

        [HttpPost]
        public async Task<ActionResult<MemberViewModel>> Create(CreateMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _memberService.CreateAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.MemberId }, result.Data);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult<MemberViewModel>> Update(long id, UpdateMemberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _memberService.UpdateAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "Member not found.")
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _memberService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{id:long}/activate")]
        public async Task<IActionResult> Activate(long id)
        {
            var activated = await _memberService.ActivateAsync(id);
            if (!activated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("{id:long}/deactivate")]
        public async Task<IActionResult> Deactivate(long id)
        {
            var deactivated = await _memberService.DeactivateAsync(id);
            if (!deactivated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id:long}/photo")]
        public async Task<ActionResult<MemberPhotoViewModel>> GetPhoto(long id)
        {
            var photo = await _memberService.GetPhotoAsync(id);
            if (photo == null)
            {
                return NotFound();
            }

            return Ok(photo);
        }

        [HttpPut("{id:long}/photo")]
        [RequestSizeLimit(2 * 1024 * 1024)]
        public async Task<ActionResult<MemberPhotoViewModel>> SavePhoto(long id, MemberPhotoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _memberService.SavePhotoAsync(id, model);
            if (!result.Succeeded)
            {
                if (result.Error == "Member not found.")
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id:long}/photo")]
        public async Task<IActionResult> DeletePhoto(long id)
        {
            var deleted = await _memberService.DeletePhotoAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
