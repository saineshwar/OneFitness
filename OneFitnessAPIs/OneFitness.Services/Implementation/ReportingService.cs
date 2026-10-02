using Microsoft.Extensions.Options;
using OneFitness.Entities;
using OneFitness.Repository.Interfaces;
using OneFitness.Services.Interfaces;
using OneFitness.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OneFitness.Services.Implementation
{
    public class ReportingService : IReportingService
    {
        private readonly IReportingRepository _reportingRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IWorkOutRepository _workOutRepository;
        private readonly IMembershipTypeRepository _membershipTypeRepository;
        private readonly IInstallmentRepository _installmentRepository;
        private readonly IEnquiryRepository _enquiryRepository;
        private readonly ITaxMasterRepository _taxMasterRepository;
        private readonly IPaymentTypeRepository _paymentTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly TallyExportOptions _tallyExportOptions;

        public ReportingService(
            IReportingRepository reportingRepository,
            IMemberRepository memberRepository,
            IWorkOutRepository workOutRepository,
            IMembershipTypeRepository membershipTypeRepository,
            IInstallmentRepository installmentRepository,
            IEnquiryRepository enquiryRepository,
            ITaxMasterRepository taxMasterRepository,
            IPaymentTypeRepository paymentTypeRepository,
            IUserRepository userRepository,
            IOptions<TallyExportOptions> tallyExportOptions)
        {
            _reportingRepository = reportingRepository;
            _memberRepository = memberRepository;
            _workOutRepository = workOutRepository;
            _membershipTypeRepository = membershipTypeRepository;
            _installmentRepository = installmentRepository;
            _enquiryRepository = enquiryRepository;
            _taxMasterRepository = taxMasterRepository;
            _paymentTypeRepository = paymentTypeRepository;
            _userRepository = userRepository;
            _tallyExportOptions = tallyExportOptions.Value;
        }

        public async Task<YearwiseReportRowViewModel> GetYearwiseReportAsync(int fiscalYear)
        {
            var (from, toExclusive) = FiscalYearRange(fiscalYear);
            var members = await _reportingRepository.GetMembersByCreatedDateRangeAsync(from, toExclusive);
            var buckets = BuildFiscalBuckets(members.Select(m => m.CreatedOn));

            return new YearwiseReportRowViewModel
            {
                FiscalYear = fiscalYear,
                April = buckets[0],
                May = buckets[1],
                June = buckets[2],
                July = buckets[3],
                August = buckets[4],
                Sept = buckets[5],
                Oct = buckets[6],
                Nov = buckets[7],
                Dec = buckets[8],
                Jan = buckets[9],
                Feb = buckets[10],
                March = buckets[11],
                Total = members.Count
            };
        }

        public async Task<IReadOnlyList<MonthlyReportMemberViewModel>> GetMonthwiseReportAsync(int year, int month)
        {
            var from = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var toExclusive = from.AddMonths(1);
            var members = await _reportingRepository.GetMembersByCreatedDateRangeAsync(from, toExclusive);
            var latestPayments = await _reportingRepository.GetLatestPaymentsByMemberIdsAsync(members.Select(m => m.MemberId).ToList());

            return members.Select(m =>
            {
                latestPayments.TryGetValue(m.MemberId, out var payment);
                return new MonthlyReportMemberViewModel
                {
                    MemberId = m.MemberId,
                    MemberNo = m.MemberNo,
                    FirstName = m.FirstName,
                    MiddleName = m.MiddleName,
                    LastName = m.LastName,
                    CreatedOn = m.CreatedOn,
                    TotalAmount = payment?.TotalAmount,
                    Status = m.Status
                };
            }).OrderByDescending(m => m.CreatedOn).ToList();
        }

        public async Task<IReadOnlyList<RenewalReportRowViewModel>> GetRenewalReportAsync(DateRangeReportRequestViewModel request)
        {
            var from = request.FromDate!.Value;
            var toExclusive = request.ToDate!.Value.AddDays(1);
            var pairs = await _reportingRepository.GetMembersWithNextRenewalInRangeAsync(from, toExclusive);

            var (workOuts, membershipTypes, installments) = await LoadLookupsAsync();

            return pairs.Select(x => new RenewalReportRowViewModel
            {
                MemberId = x.Member.MemberId,
                MemberNo = x.Member.MemberNo,
                FullName = FormatFullName(x.Member),
                InstallmentName = installments.TryGetValue(x.Payment.InstallmentId, out var installmentName) ? installmentName : null,
                MembershipTypeName = membershipTypes.TryGetValue(x.Payment.MembershipTypeId, out var membershipTypeName) ? membershipTypeName : null,
                WorkOutName = workOuts.TryGetValue(x.Payment.WorkOutId, out var workOutName) ? workOutName : null,
                JoiningDate = x.Member.JoiningDate,
                NextRenewalDate = x.Payment.NextRenewalDate,
                TotalAmount = x.Payment.TotalAmount,
                MobileNo = x.Member.MobileNo ?? string.Empty,
                EmailId = x.Member.EmailId ?? string.Empty,
                Address = x.Member.Address
            }).OrderBy(m => m.NextRenewalDate).ToList();
        }

        public async Task<IReadOnlyList<MemberJoinedRowViewModel>> GetJoinedReportAsync(DateRangeReportRequestViewModel request)
        {
            var from = request.FromDate!.Value;
            var toExclusive = request.ToDate!.Value.AddDays(1);
            var members = await _reportingRepository.GetMembersByJoiningDateRangeAsync(from, toExclusive);
            var firstPayments = await _reportingRepository.GetFirstPaymentsByMemberIdsAsync(members.Select(m => m.MemberId).ToList());

            var (workOuts, membershipTypes, installments) = await LoadLookupsAsync();

            return members.Select(m =>
            {
                firstPayments.TryGetValue(m.MemberId, out var payment);
                return new MemberJoinedRowViewModel
                {
                    MemberId = m.MemberId,
                    MemberNo = m.MemberNo,
                    FullName = FormatFullName(m),
                    MobileNo = m.MobileNo ?? string.Empty,
                    EmailId = m.EmailId ?? string.Empty,
                    WorkOutName = payment != null && workOuts.TryGetValue(payment.WorkOutId, out var workOutName) ? workOutName : null,
                    MembershipTypeName = payment != null && membershipTypes.TryGetValue(payment.MembershipTypeId, out var membershipTypeName) ? membershipTypeName : null,
                    InstallmentName = payment != null && installments.TryGetValue(payment.InstallmentId, out var installmentName) ? installmentName : null,
                    JoiningDate = m.JoiningDate,
                    TotalAmount = payment?.TotalAmount,
                    Status = m.Status
                };
            }).OrderBy(m => m.JoiningDate).ToList();
        }

        public async Task<IReadOnlyList<RefundReportRowViewModel>> GetRefundReportAsync(DateRangeReportRequestViewModel request)
        {
            var from = request.FromDate!.Value;
            var toExclusive = request.ToDate!.Value.AddDays(1);
            var refunds = await _reportingRepository.GetRefundsByDateRangeAsync(from, toExclusive);

            var (workOuts, membershipTypes, installments) = await LoadLookupsAsync();
            var latestPayments = await _reportingRepository.GetLatestPaymentsByMemberIdsAsync(refunds.Select(r => r.MemberId).Distinct().ToList());
            var result = new List<RefundReportRowViewModel>();

            foreach (var refund in refunds)
            {
                var member = await _memberRepository.GetByIdAsync(refund.MemberId);
                if (member == null)
                {
                    continue;
                }

                latestPayments.TryGetValue(refund.MemberId, out var payment);

                result.Add(new RefundReportRowViewModel
                {
                    RefundId = refund.RefundId,
                    MemberId = member.MemberId,
                    MemberNo = member.MemberNo,
                    FullName = FormatFullName(member),
                    MobileNo = member.MobileNo ?? string.Empty,
                    EmailId = member.EmailId ?? string.Empty,
                    InstallmentName = payment != null && installments.TryGetValue(payment.InstallmentId, out var installmentName) ? installmentName : null,
                    MembershipTypeName = payment != null && membershipTypes.TryGetValue(payment.MembershipTypeId, out var membershipTypeName) ? membershipTypeName : null,
                    WorkOutName = payment != null && workOuts.TryGetValue(payment.WorkOutId, out var workOutName) ? workOutName : null,
                    JoiningDate = member.JoiningDate,
                    SubscriptionAmount = payment?.TotalAmount,
                    RefundAmount = refund.Amount,
                    RefundedDate = refund.CreatedOn
                });
            }

            return result.OrderByDescending(r => r.RefundedDate).ToList();
        }

        public async Task<IReadOnlyList<IncomeCreditDebitRowViewModel>> GetIncomeCreditDebitReportAsync(DateRangeReportRequestViewModel request)
        {
            var from = request.FromDate!.Value;
            var toExclusive = request.ToDate!.Value.AddDays(1);

            var receipts = await _reportingRepository.GetReceiptHistoriesByDateRangeAsync(from, toExclusive);
            var refunds = await _reportingRepository.GetRefundsByDateRangeAsync(from, toExclusive);

            var rows = new List<IncomeCreditDebitRowViewModel>();

            foreach (var receipt in receipts)
            {
                var member = await _memberRepository.GetByMemberNoAsync(receipt.MemberNo);
                rows.Add(new IncomeCreditDebitRowViewModel
                {
                    Date = receipt.CreatedOn,
                    VoucherType = "Receipt",
                    Particulars = member != null ? FormatFullName(member) : receipt.MemberNo,
                    MemberNo = receipt.MemberNo,
                    InvoiceNo = receipt.InvoiceNo,
                    CreditAmount = receipt.TotalAmount ?? 0,
                    DebitAmount = 0
                });
            }

            foreach (var refund in refunds)
            {
                var member = await _memberRepository.GetByIdAsync(refund.MemberId);
                rows.Add(new IncomeCreditDebitRowViewModel
                {
                    Date = refund.CreatedOn,
                    VoucherType = "Refund",
                    Particulars = member != null ? FormatFullName(member) : refund.MemberId.ToString(),
                    MemberNo = member?.MemberNo ?? string.Empty,
                    InvoiceNo = null,
                    CreditAmount = 0,
                    DebitAmount = refund.Amount
                });
            }

            return rows.OrderBy(r => r.Date).ToList();
        }

        public async Task<IReadOnlyList<TaxSummaryRowViewModel>> GetTaxSummaryReportAsync(DateRangeReportRequestViewModel request)
        {
            var from = request.FromDate!.Value;
            var toExclusive = request.ToDate!.Value.AddDays(1);

            var receipts = await _reportingRepository.GetReceiptHistoriesByDateRangeAsync(from, toExclusive);
            var taxMasters = (await _taxMasterRepository.GetAllAsync()).ToDictionary(t => t.TaxId, t => t);

            return receipts
                .GroupBy(r => r.TaxId)
                .Select(g =>
                {
                    var taxMaster = g.Key.HasValue && taxMasters.TryGetValue(g.Key.Value, out var tm) ? tm : null;
                    return new TaxSummaryRowViewModel
                    {
                        TaxId = g.Key,
                        TaxType = taxMaster?.TaxType ?? "Unspecified",
                        TaxRate = taxMaster?.TaxRate ?? 0,
                        TransactionCount = g.Count(),
                        TaxableAmount = g.Sum(r => r.Amount ?? 0),
                        TaxAmount = g.Sum(r => r.TaxPercentageAmount ?? 0),
                        TotalAmount = g.Sum(r => r.TotalAmount ?? 0)
                    };
                })
                .OrderBy(r => r.TaxType)
                .ToList();
        }

        public async Task<IReadOnlyList<PaymentTypeCollectionRowViewModel>> GetPaymentTypeCollectionReportAsync(DateRangeReportRequestViewModel request)
        {
            var from = request.FromDate!.Value;
            var toExclusive = request.ToDate!.Value.AddDays(1);

            var receipts = await _reportingRepository.GetReceiptHistoriesByDateRangeAsync(from, toExclusive);
            var paymentTypes = (await _paymentTypeRepository.GetAllAsync()).ToDictionary(p => p.PaymentTypeId, p => p.PaymentTypeName);

            return receipts
                .GroupBy(r => r.PaymentTypeId)
                .Select(g => new PaymentTypeCollectionRowViewModel
                {
                    PaymentTypeId = g.Key,
                    PaymentTypeName = g.Key.HasValue && paymentTypes.TryGetValue(g.Key.Value, out var name) ? name : "Unspecified",
                    TransactionCount = g.Count(),
                    TotalAmount = g.Sum(r => r.TotalAmount ?? 0)
                })
                .OrderByDescending(r => r.TotalAmount)
                .ToList();
        }

        public async Task<IReadOnlyList<StaffCollectionRowViewModel>> GetStaffCollectionReportAsync(DateRangeReportRequestViewModel request)
        {
            var from = request.FromDate!.Value;
            var toExclusive = request.ToDate!.Value.AddDays(1);

            var receipts = await _reportingRepository.GetReceiptHistoriesByDateRangeAsync(from, toExclusive);
            var refunds = await _reportingRepository.GetRefundsByDateRangeAsync(from, toExclusive);
            var users = (await _userRepository.GetAllAsync()).ToDictionary(u => u.UserId, u => u);

            var staffIds = receipts.Select(r => r.CreatedBy)
                .Concat(refunds.Select(r => r.CreatedBy))
                .Distinct();

            var rows = new List<StaffCollectionRowViewModel>();
            foreach (var staffId in staffIds)
            {
                var staffReceipts = receipts.Where(r => r.CreatedBy == staffId).ToList();
                var staffRefunds = refunds.Where(r => r.CreatedBy == staffId).ToList();
                var user = staffId.HasValue && users.TryGetValue(staffId.Value, out var u) ? u : null;

                rows.Add(new StaffCollectionRowViewModel
                {
                    UserId = staffId,
                    StaffName = user != null ? FormatUserName(user) : "Unassigned",
                    ReceiptCount = staffReceipts.Count,
                    ReceiptTotal = staffReceipts.Sum(r => r.TotalAmount ?? 0),
                    RefundCount = staffRefunds.Count,
                    RefundTotal = staffRefunds.Sum(r => r.Amount)
                });
            }

            return rows.OrderByDescending(r => r.ReceiptTotal).ToList();
        }

        public async Task<IReadOnlyList<RenewalStatusRowViewModel>> GetRenewalStatusReportAsync(int daysAhead)
        {
            var today = DateTime.UtcNow.Date;
            var dueSoonToExclusive = today.AddDays(daysAhead + 1);

            var dueSoonPairs = await _reportingRepository.GetActiveMembersWithNextRenewalInRangeAsync(today, dueSoonToExclusive);
            var lapsedPairs = await _reportingRepository.GetActiveMembersWithNextRenewalBeforeAsync(today);

            var (workOuts, membershipTypes, _) = await LoadLookupsAsync();

            IEnumerable<RenewalStatusRowViewModel> ToRows(IEnumerable<(Member Member, Payment Payment)> pairs, string statusLabel)
            {
                return pairs.Select(x => new RenewalStatusRowViewModel
                {
                    MemberId = x.Member.MemberId,
                    MemberNo = x.Member.MemberNo,
                    FullName = FormatFullName(x.Member),
                    MobileNo = x.Member.MobileNo ?? string.Empty,
                    EmailId = x.Member.EmailId ?? string.Empty,
                    WorkOutName = workOuts.TryGetValue(x.Payment.WorkOutId, out var workOutName) ? workOutName : null,
                    MembershipTypeName = membershipTypes.TryGetValue(x.Payment.MembershipTypeId, out var membershipTypeName) ? membershipTypeName : null,
                    NextRenewalDate = x.Payment.NextRenewalDate,
                    DaysRemaining = (int)(x.Payment.NextRenewalDate.Date - today).TotalDays,
                    StatusLabel = statusLabel
                });
            }

            var rows = ToRows(lapsedPairs, "Lapsed").Concat(ToRows(dueSoonPairs, "Due Soon"));
            return rows.OrderBy(r => r.NextRenewalDate).ToList();
        }

        public async Task<IReadOnlyList<OutstandingBalanceRowViewModel>> GetOutstandingBalancesReportAsync()
        {
            var payments = await _reportingRepository.GetPaymentsWithOutstandingBalanceAsync();
            var (workOuts, membershipTypes, installments) = await LoadLookupsAsync();
            var result = new List<OutstandingBalanceRowViewModel>();

            foreach (var payment in payments)
            {
                var member = await _memberRepository.GetByIdAsync(payment.MemberId);
                if (member == null)
                {
                    continue;
                }

                result.Add(new OutstandingBalanceRowViewModel
                {
                    PaymentId = payment.PaymentId,
                    MemberId = member.MemberId,
                    MemberNo = member.MemberNo,
                    FullName = FormatFullName(member),
                    MobileNo = member.MobileNo ?? string.Empty,
                    EmailId = member.EmailId ?? string.Empty,
                    MembershipTypeName = membershipTypes.TryGetValue(payment.MembershipTypeId, out var membershipTypeName) ? membershipTypeName : null,
                    WorkOutName = workOuts.TryGetValue(payment.WorkOutId, out var workOutName) ? workOutName : null,
                    InstallmentName = installments.TryGetValue(payment.InstallmentId, out var installmentName) ? installmentName : null,
                    InvoiceNo = payment.InvoiceNo,
                    TotalAmount = payment.TotalAmount,
                    AmountPaid = payment.AmountPaid,
                    BalanceDue = payment.TotalAmount - payment.AmountPaid,
                    PaymentDate = payment.CreatedOn,
                    NextRenewalDate = payment.NextRenewalDate
                });
            }

            return result.OrderByDescending(r => r.BalanceDue).ToList();
        }

        public async Task<byte[]> GetTallyExportXmlAsync(DateRangeReportRequestViewModel request)
        {
            var from = request.FromDate!.Value;
            var toExclusive = request.ToDate!.Value.AddDays(1);
            var options = _tallyExportOptions;

            var receipts = await _reportingRepository.GetReceiptHistoriesByDateRangeAsync(from, toExclusive);
            var refunds = await _reportingRepository.GetRefundsByDateRangeAsync(from, toExclusive);

            var messages = new List<XElement>();

            foreach (var receipt in receipts.OrderBy(r => r.CreatedOn))
            {
                var baseAmount = receipt.Amount ?? 0;
                var taxAmount = receipt.TaxPercentageAmount ?? 0;
                var total = baseAmount + taxAmount;
                if (total <= 0)
                {
                    continue;
                }

                var ledgerEntries = new List<XElement>
                {
                    BuildLedgerEntry(options.CashLedger, isDebit: true, total),
                    BuildLedgerEntry(options.IncomeLedger, isDebit: false, baseAmount)
                };

                if (taxAmount > 0)
                {
                    ledgerEntries.Add(BuildLedgerEntry(options.GstLedger, isDebit: false, taxAmount));
                }

                messages.Add(BuildVoucherMessage(
                    options.ReceiptVoucherType,
                    receipt.CreatedOn,
                    receipt.InvoiceNo?.ToString(CultureInfo.InvariantCulture) ?? receipt.ReceiptHistoryId.ToString(CultureInfo.InvariantCulture),
                    $"Membership payment - {receipt.MemberNo}",
                    options.CashLedger,
                    ledgerEntries));
            }

            foreach (var refund in refunds.OrderBy(r => r.CreatedOn))
            {
                if (refund.Amount <= 0)
                {
                    continue;
                }

                var member = await _memberRepository.GetByIdAsync(refund.MemberId);
                var memberNo = member?.MemberNo ?? refund.MemberId.ToString(CultureInfo.InvariantCulture);

                var ledgerEntries = new List<XElement>
                {
                    BuildLedgerEntry(options.RefundLedger, isDebit: true, refund.Amount),
                    BuildLedgerEntry(options.CashLedger, isDebit: false, refund.Amount)
                };

                messages.Add(BuildVoucherMessage(
                    options.PaymentVoucherType,
                    refund.CreatedOn,
                    refund.RefundId.ToString(CultureInfo.InvariantCulture),
                    $"Membership refund - {memberNo}",
                    options.CashLedger,
                    ledgerEntries));
            }

            var envelope = new XElement("ENVELOPE",
                new XElement("HEADER",
                    new XElement("TALLYREQUEST", "Import Data")),
                new XElement("BODY",
                    new XElement("IMPORTDATA",
                        new XElement("REQUESTDESC",
                            new XElement("REPORTNAME", "Vouchers"),
                            new XElement("STATICVARIABLES",
                                new XElement("SVCURRENTCOMPANY", options.CompanyName))),
                        new XElement("REQUESTDATA", messages))));

            var document = new XDocument(new XDeclaration("1.0", "UTF-8", null), envelope);

            using var stream = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
            {
                document.Save(xmlWriter);
            }

            return stream.ToArray();
        }

        private static XElement BuildLedgerEntry(string ledgerName, bool isDebit, decimal amount)
        {
            var signedAmount = isDebit ? -amount : amount;
            return new XElement("ALLLEDGERENTRIES.LIST",
                new XElement("LEDGERNAME", ledgerName),
                new XElement("ISDEEMEDPOSITIVE", isDebit ? "Yes" : "No"),
                new XElement("AMOUNT", signedAmount.ToString("0.00", CultureInfo.InvariantCulture)));
        }

        private static XElement BuildVoucherMessage(
            string voucherType,
            DateTime date,
            string voucherNumber,
            string narration,
            string partyLedgerName,
            IEnumerable<XElement> ledgerEntries)
        {
            var voucher = new XElement("VOUCHER",
                new XAttribute("VCHTYPE", voucherType),
                new XAttribute("ACTION", "Create"),
                new XElement("DATE", date.ToString("yyyyMMdd", CultureInfo.InvariantCulture)),
                new XElement("NARRATION", narration),
                new XElement("VOUCHERTYPENAME", voucherType),
                new XElement("VOUCHERNUMBER", voucherNumber),
                new XElement("PARTYLEDGERNAME", partyLedgerName));

            voucher.Add(ledgerEntries);

            return new XElement("TALLYMESSAGE",
                new XAttribute(XNamespace.Xmlns + "UDF", "TallyUDF"),
                voucher);
        }

        public async Task<DashboardSummaryViewModel> GetDashboardSummaryAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var startOfNextMonth = startOfMonth.AddMonths(1);
            var startOfToday = now.Date;
            var startOfTomorrow = startOfToday.AddDays(1);
            var currentFiscalYear = now.Month >= 4 ? now.Year : now.Year - 1;
            var (fiscalFrom, fiscalToExclusive) = FiscalYearRange(currentFiscalYear);

            var newThisMonth = await _reportingRepository.CountMembersByCreatedDateRangeAsync(startOfMonth, startOfNextMonth);
            var newToday = await _reportingRepository.CountMembersByCreatedDateRangeAsync(startOfToday, startOfTomorrow);
            var renewedThisMonth = await _reportingRepository.CountRenewalPaymentsByDateRangeAsync(startOfMonth, startOfNextMonth);
            var refundsThisMonth = await _reportingRepository.CountRefundsByDateRangeAsync(startOfMonth, startOfNextMonth);
            var enquiryCount = (await _enquiryRepository.GetAllAsync()).Count;
            var totalMembers = await _reportingRepository.CountAllMembersAsync();
            var activeMembers = await _reportingRepository.CountActiveMembersAsync();
            var monthlyRevenue = await _reportingRepository.SumReceiptTotalByDateRangeAsync(startOfMonth, startOfNextMonth);

            var fiscalYearMembers = await _reportingRepository.GetMembersByCreatedDateRangeAsync(fiscalFrom, fiscalToExclusive);
            var fiscalYearRenewedPayments = await _reportingRepository.GetRenewalPaymentsByDateRangeAsync(fiscalFrom, fiscalToExclusive);
            var fiscalYearPayments = await _reportingRepository.GetPaymentsByDateRangeAsync(fiscalFrom, fiscalToExclusive);

            var newBuckets = BuildFiscalBuckets(fiscalYearMembers.Select(m => m.CreatedOn));
            var renewedBuckets = BuildFiscalBuckets(fiscalYearRenewedPayments.Select(p => p.CreatedOn));

            var membershipTypeLookup = (await _membershipTypeRepository.GetAllAsync()).ToDictionary(mt => mt.MembershipTypeId, mt => mt.MembershipTypeName);
            var topMembershipTypes = fiscalYearPayments
                .GroupBy(p => p.MembershipTypeId)
                .Select(g => new TopMembershipTypeViewModel
                {
                    MembershipTypeId = g.Key,
                    MembershipTypeName = membershipTypeLookup.TryGetValue(g.Key, out var name) ? name : string.Empty,
                    TotalCount = g.Count()
                })
                .OrderByDescending(t => t.TotalCount)
                .Take(5)
                .ToList();

            return new DashboardSummaryViewModel
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                MonthlyRevenue = monthlyRevenue,
                NewRegistrationsThisMonth = newThisMonth,
                NewRegistrationsToday = newToday,
                RenewedThisMonth = renewedThisMonth,
                RefundsThisMonth = refundsThisMonth,
                EnquiryCount = enquiryCount,
                YearwiseNewChart = ToChartViewModel(newBuckets),
                YearwiseRenewedChart = ToChartViewModel(renewedBuckets),
                TopMembershipTypes = topMembershipTypes
            };
        }

        private static (DateTime From, DateTime ToExclusive) FiscalYearRange(int fiscalYear)
        {
            var from = new DateTime(fiscalYear, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            var toExclusive = new DateTime(fiscalYear + 1, 4, 1, 0, 0, 0, DateTimeKind.Utc);
            return (from, toExclusive);
        }

        private static int[] BuildFiscalBuckets(IEnumerable<DateTime> dates)
        {
            var buckets = new int[12];
            foreach (var date in dates)
            {
                var slot = (date.Month + 8) % 12;
                buckets[slot]++;
            }

            return buckets;
        }

        private static YearwiseChartViewModel ToChartViewModel(int[] buckets)
        {
            return new YearwiseChartViewModel
            {
                April = buckets[0],
                May = buckets[1],
                June = buckets[2],
                July = buckets[3],
                August = buckets[4],
                Sept = buckets[5],
                Oct = buckets[6],
                Nov = buckets[7],
                Dec = buckets[8],
                Jan = buckets[9],
                Feb = buckets[10],
                March = buckets[11]
            };
        }

        private async Task<(Dictionary<int, string> WorkOuts, Dictionary<int, string> MembershipTypes, Dictionary<int, string> Installments)> LoadLookupsAsync()
        {
            var workOuts = (await _workOutRepository.GetAllAsync()).ToDictionary(w => w.WorkOutId, w => w.WorkOutName);
            var membershipTypes = (await _membershipTypeRepository.GetAllAsync()).ToDictionary(mt => mt.MembershipTypeId, mt => mt.MembershipTypeName);
            var installments = (await _installmentRepository.GetAllAsync()).ToDictionary(i => i.InstallmentId, i => i.InstallmentName);
            return (workOuts, membershipTypes, installments);
        }

        private static string FormatFullName(Member member)
        {
            return string.Join(" ", new[] { member.FirstName, member.MiddleName, member.LastName }.Where(n => !string.IsNullOrWhiteSpace(n)));
        }

        private static string FormatUserName(User user)
        {
            return string.Join(" ", new[] { user.FirstName, user.LastName }.Where(n => !string.IsNullOrWhiteSpace(n)));
        }
    }
}
