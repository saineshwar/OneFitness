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
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpGet("history")]
        public async Task<ActionResult<IReadOnlyList<ReceiptHistoryViewModel>>> GetAllHistory()
        {
            var history = await _receiptService.GetAllHistoryAsync();
            return Ok(history);
        }

        [HttpGet("history/paged")]
        public async Task<ActionResult<PagedResultViewModel<ReceiptHistoryViewModel>>> GetPagedHistory(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _receiptService.GetPagedHistoryAsync(page, pageSize, search);
            return Ok(result);
        }

        [HttpGet("history/{receiptHistoryId:long}")]
        public async Task<ActionResult<ReceiptViewModel>> GetHistoryDetail(long receiptHistoryId)
        {
            var result = await _receiptService.GetHistoryDetailAsync(receiptHistoryId);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return Ok(result.Data);
        }

        [HttpPost("generate/{memberId:long}")]
        public async Task<ActionResult<ReceiptViewModel>> Generate(long memberId)
        {
            var result = await _receiptService.GenerateAsync(memberId, this.GetCurrentUserId());
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
    }
}
