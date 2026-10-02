using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneFitness.API.Extensions;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneFitness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RefundController : ControllerBase
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<RefundViewModel>>> GetAll()
        {
            var refunds = await _refundService.GetAllAsync();
            return Ok(refunds);
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultViewModel<RefundViewModel>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _refundService.GetPagedAsync(page, pageSize, search);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RefundViewModel>> GetById(int id)
        {
            var refund = await _refundService.GetByIdAsync(id);
            if (refund == null)
            {
                return NotFound();
            }

            return Ok(refund);
        }

        [HttpPost]
        public async Task<ActionResult<RefundViewModel>> Create(CreateRefundViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _refundService.CreateAsync(model, this.GetCurrentUserId());
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.RefundId }, result.Data);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _refundService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
