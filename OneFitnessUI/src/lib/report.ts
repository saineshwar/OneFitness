import { api, API_BASE_URL, ApiError } from "@/lib/api";

export type YearwiseReportRow = {
  fiscalYear: number;
  april: number;
  may: number;
  june: number;
  july: number;
  august: number;
  sept: number;
  oct: number;
  nov: number;
  dec: number;
  jan: number;
  feb: number;
  march: number;
  total: number;
};

export type MonthlyReportMember = {
  memberId: number;
  memberNo: string;
  firstName: string;
  middleName: string | null;
  lastName: string | null;
  createdOn: string;
  totalAmount: number | null;
  status: boolean;
};

export type RenewalReportRow = {
  memberId: number;
  memberNo: string;
  fullName: string;
  installmentName: string | null;
  membershipTypeName: string | null;
  workOutName: string | null;
  joiningDate: string | null;
  nextRenewalDate: string | null;
  totalAmount: number | null;
  mobileNo: string;
  emailId: string;
  address: string;
};

export type MemberJoinedRow = {
  memberId: number;
  memberNo: string;
  fullName: string;
  mobileNo: string;
  emailId: string;
  workOutName: string | null;
  membershipTypeName: string | null;
  installmentName: string | null;
  joiningDate: string | null;
  totalAmount: number | null;
  status: boolean;
};

export type RefundReportRow = {
  refundId: number;
  memberId: number;
  memberNo: string;
  fullName: string;
  mobileNo: string;
  emailId: string;
  installmentName: string | null;
  membershipTypeName: string | null;
  workOutName: string | null;
  joiningDate: string | null;
  subscriptionAmount: number | null;
  refundAmount: number;
  refundedDate: string;
};

export type IncomeCreditDebitRow = {
  date: string;
  voucherType: string;
  particulars: string;
  memberNo: string;
  invoiceNo: number | null;
  creditAmount: number;
  debitAmount: number;
};

export type TaxSummaryRow = {
  taxId: number | null;
  taxType: string;
  taxRate: number;
  transactionCount: number;
  taxableAmount: number;
  taxAmount: number;
  totalAmount: number;
};

export type PaymentTypeCollectionRow = {
  paymentTypeId: number | null;
  paymentTypeName: string;
  transactionCount: number;
  totalAmount: number;
};

export type StaffCollectionRow = {
  userId: number | null;
  staffName: string;
  receiptCount: number;
  receiptTotal: number;
  refundCount: number;
  refundTotal: number;
};

export type RenewalStatusRow = {
  memberId: number;
  memberNo: string;
  fullName: string;
  mobileNo: string;
  emailId: string;
  workOutName: string | null;
  membershipTypeName: string | null;
  nextRenewalDate: string | null;
  daysRemaining: number;
  statusLabel: string;
};

export type OutstandingBalanceRow = {
  paymentId: number;
  memberId: number;
  memberNo: string;
  fullName: string;
  mobileNo: string;
  emailId: string;
  membershipTypeName: string | null;
  workOutName: string | null;
  installmentName: string | null;
  invoiceNo: number;
  totalAmount: number;
  amountPaid: number;
  balanceDue: number;
  paymentDate: string;
  nextRenewalDate: string;
};

export type DateRangeReportRequest = {
  fromDate: string;
  toDate: string;
};

export function getYearwiseReport(fiscalYear: number) {
  return api.get<YearwiseReportRow>(`/Report/yearwise?fiscalYear=${fiscalYear}`);
}

export function getMonthwiseReport(year: number, month: number) {
  return api.get<MonthlyReportMember[]>(`/Report/monthwise?year=${year}&month=${month}`);
}

export function getRenewalReport(request: DateRangeReportRequest) {
  return api.post<RenewalReportRow[]>("/Report/renewal", request);
}

export function getJoinedReport(request: DateRangeReportRequest) {
  return api.post<MemberJoinedRow[]>("/Report/joined", request);
}

export function getRefundReport(request: DateRangeReportRequest) {
  return api.post<RefundReportRow[]>("/Report/refund", request);
}

export function getIncomeCreditDebitReport(request: DateRangeReportRequest) {
  return api.post<IncomeCreditDebitRow[]>("/Report/income-credit-debit", request);
}

export function getTaxSummaryReport(request: DateRangeReportRequest) {
  return api.post<TaxSummaryRow[]>("/Report/tax-summary", request);
}

export function getPaymentTypeCollectionReport(request: DateRangeReportRequest) {
  return api.post<PaymentTypeCollectionRow[]>("/Report/payment-type-collection", request);
}

export function getStaffCollectionReport(request: DateRangeReportRequest) {
  return api.post<StaffCollectionRow[]>("/Report/staff-collection", request);
}

export function getRenewalStatusReport(daysAhead: number) {
  return api.get<RenewalStatusRow[]>(`/Report/renewal-status?daysAhead=${daysAhead}`);
}

export function getOutstandingBalancesReport() {
  return api.get<OutstandingBalanceRow[]>("/Report/outstanding-balances");
}

export async function downloadTallyExport(request: DateRangeReportRequest) {
  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}/Report/tally-export`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(request),
    });
  } catch {
    throw new ApiError("Could not reach the API. Is OneFitness.API running?", 0);
  }

  if (!response.ok) {
    throw new ApiError(`Failed to generate Tally export (status ${response.status}).`, response.status);
  }

  const blob = await response.blob();
  const disposition = response.headers.get("Content-Disposition");
  const match = disposition?.match(/filename="?([^";]+)"?/);
  const filename = match?.[1] ?? `tally-export_${request.fromDate}_${request.toDate}.xml`;

  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  link.click();
  URL.revokeObjectURL(url);
}
