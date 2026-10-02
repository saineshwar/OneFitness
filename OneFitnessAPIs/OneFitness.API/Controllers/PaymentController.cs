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
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet("paged")]
        public async Task<ActionResult<PagedResultViewModel<PaymentViewModel>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _paymentService.GetPagedAsync(page, pageSize, search);
            return Ok(result);
        }

        [HttpGet("member/{memberId:long}")]
        public async Task<ActionResult<IReadOnlyList<PaymentViewModel>>> GetByMember(long memberId)
        {
            var payments = await _paymentService.GetByMemberIdAsync(memberId);
            return Ok(payments);
        }

        [HttpGet("member/{memberId:long}/latest")]
        public async Task<ActionResult<PaymentViewModel>> GetLatestByMember(long memberId)
        {
            var payment = await _paymentService.GetLatestByMemberIdAsync(memberId);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<PaymentViewModel>> GetById(long id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpPost("calculate-amount")]
        public async Task<ActionResult<PaymentAmountCalculationViewModel>> CalculateAmount(CalculatePaymentAmountViewModel model)
        {
            var result = await _paymentService.CalculateAmountAsync(model);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<PaymentViewModel>> Create(CreatePaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _paymentService.CreatePaymentAsync(model, this.GetCurrentUserId());
            if (!result.Succeeded)
            {
                if (result.Error == "Member not found.")
                {
                    return NotFound();
                }

                ModelState.AddModelError(string.Empty, result.Error!);
                return ValidationProblem(ModelState);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.PaymentId }, result.Data);
        }

        [HttpPost("{id:long}/collect")]
        public async Task<ActionResult<PaymentViewModel>> CollectBalance(long id, CollectPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _paymentService.CollectBalanceAsync(id, model, this.GetCurrentUserId());
            if (!result.Succeeded)
            {
                if (result.Error is "Payment not found." or "Member not found.")
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
