import { api, type PagedResult } from "@/lib/api";

export type Receipt = {
  memberId: number;
  memberNo: string;
  firstName: string;
  middleName: string | null;
  lastName: string | null;
  paymentFromDate: string | null;
  nextRenewalDate: string | null;
  installmentName: string | null;
  membershipTypeName: string | null;
  workOutName: string | null;
  paymentTypeName: string | null;
  taxRate: number | null;
  taxType: string | null;
  identificationNo: string | null;
  amount: number | null;
  taxPercentage: number | null;
  taxPercentageAmount: number | null;
  totalAmount: number | null;
  amountPaid: number | null;
  balanceDue: number | null;
  invoiceNo: number | null;
  invoiceDate: string;
  companyName: string | null;
  companyAddress: string | null;
  companyLogoPath: string | null;
  companySupportEmailId: string | null;
  companyTelephoneNo: string | null;
};

export type ReceiptHistory = {
  receiptHistoryId: number;
  invoiceNo: number | null;
  memberNo: string;
  createdOn: string;
  createdBy: number | null;
};

export function generateReceipt(memberId: number) {
  return api.post<Receipt>(`/Receipt/generate/${memberId}`, undefined);
}

export function getReceiptById(receiptHistoryId: number) {
  return api.get<Receipt>(`/Receipt/history/${receiptHistoryId}`);
}

export function getReceiptHistory() {
  return api.get<ReceiptHistory[]>("/Receipt/history");
}

export function getReceiptHistoryPaged(params: { page: number; pageSize: number; search?: string }) {
  const query = new URLSearchParams({
    page: String(params.page),
    pageSize: String(params.pageSize),
  });
  if (params.search) {
    query.set("search", params.search);
  }
  return api.get<PagedResult<ReceiptHistory>>(`/Receipt/history/paged?${query.toString()}`);
}
