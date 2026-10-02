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
    public class ReportController : ControllerBase
    {
        private readonly IReportingService _reportingService;

        public ReportController(IReportingService reportingService)
        {
            _reportingService = reportingService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("yearwise")]
        public async Task<ActionResult<YearwiseReportRowViewModel>> Yearwise([FromQuery] int fiscalYear)
        {
            var report = await _reportingService.GetYearwiseReportAsync(fiscalYear);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("monthwise")]
        public async Task<ActionResult<IReadOnlyList<MonthlyReportMemberViewModel>>> Monthwise([FromQuery] int year, [FromQuery] int month)
        {
            var report = await _reportingService.GetMonthwiseReportAsync(year, month);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("renewal")]
        public async Task<ActionResult<IReadOnlyList<RenewalReportRowViewModel>>> Renewal(DateRangeReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var report = await _reportingService.GetRenewalReportAsync(request);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("joined")]
        public async Task<ActionResult<IReadOnlyList<MemberJoinedRowViewModel>>> Joined(DateRangeReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var report = await _reportingService.GetJoinedReportAsync(request);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("refund")]
        public async Task<ActionResult<IReadOnlyList<RefundReportRowViewModel>>> Refund(DateRangeReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var report = await _reportingService.GetRefundReportAsync(request);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("income-credit-debit")]
        public async Task<ActionResult<IReadOnlyList<IncomeCreditDebitRowViewModel>>> IncomeCreditDebit(DateRangeReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var report = await _reportingService.GetIncomeCreditDebitReportAsync(request);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("tax-summary")]
        public async Task<ActionResult<IReadOnlyList<TaxSummaryRowViewModel>>> TaxSummary(DateRangeReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var report = await _reportingService.GetTaxSummaryReportAsync(request);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("payment-type-collection")]
        public async Task<ActionResult<IReadOnlyList<PaymentTypeCollectionRowViewModel>>> PaymentTypeCollection(DateRangeReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var report = await _reportingService.GetPaymentTypeCollectionReportAsync(request);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("staff-collection")]
        public async Task<ActionResult<IReadOnlyList<StaffCollectionRowViewModel>>> StaffCollection(DateRangeReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var report = await _reportingService.GetStaffCollectionReportAsync(request);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("outstanding-balances")]
        public async Task<ActionResult<IReadOnlyList<OutstandingBalanceRowViewModel>>> OutstandingBalances()
        {
            var report = await _reportingService.GetOutstandingBalancesReportAsync();
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("renewal-status")]
        public async Task<ActionResult<IReadOnlyList<RenewalStatusRowViewModel>>> RenewalStatus([FromQuery] int daysAhead = 7)
        {
            var report = await _reportingService.GetRenewalStatusReportAsync(daysAhead);
            return Ok(report);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("tally-export")]
        public async Task<IActionResult> TallyExport(DateRangeReportRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var bytes = await _reportingService.GetTallyExportXmlAsync(request);
            var fileName = $"tally-export_{request.FromDate:yyyyMMdd}_{request.ToDate:yyyyMMdd}.xml";
            return File(bytes, "application/xml", fileName);
        }

        [HttpGet("dashboard-summary")]
        public async Task<ActionResult<DashboardSummaryViewModel>> DashboardSummary()
        {
            var summary = await _reportingService.GetDashboardSummaryAsync();
            return Ok(summary);
        }
    }
}
