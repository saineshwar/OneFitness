import { api, type PagedResult } from "@/lib/api";

export type Payment = {
  paymentId: number;
  memberId: number;
  memberNo: string;
  memberFullName: string;
  workOutId: number;
  membershipTypeId: number;
  installmentId: number;
  paymentTypeId: number;
  taxId: number;
  amount: number;
  taxPercentage: number;
  taxPercentageAmount: number;
  totalAmount: number;
  amountPaid: number;
  balanceDue: number;
  invoiceNo: number;
  paymentFromDate: string;
  nextRenewalDate: string;
  createdOn: string;
};

export type PaymentInput = {
  memberId: number;
  workOutId: number;
  membershipTypeId: number;
  installmentId: number;
  paymentTypeId: number;
  taxId: number;
  amountPaid?: number;
};

export type PaymentAmountCalculation = {
  amount: number;
  taxPercentage: number;
  taxPercentageAmount: number;
  totalAmount: number;
};

export function getPaymentsPaged(params: { page: number; pageSize: number; search?: string }) {
  const query = new URLSearchParams({
    page: String(params.page),
    pageSize: String(params.pageSize),
  });
  if (params.search) {
    query.set("search", params.search);
  }
  return api.get<PagedResult<Payment>>(`/Payment/paged?${query.toString()}`);
}

export function getPaymentsByMember(memberId: number) {
  return api.get<Payment[]>(`/Payment/member/${memberId}`);
}

export function getLatestPaymentByMember(memberId: number) {
  return api.get<Payment>(`/Payment/member/${memberId}/latest`);
}

export function getPaymentById(paymentId: number) {
  return api.get<Payment>(`/Payment/${paymentId}`);
}

export function calculatePaymentAmount(membershipTypeId: number, taxId: number) {
  return api.post<PaymentAmountCalculation>("/Payment/calculate-amount", { membershipTypeId, taxId });
}

export function createPayment(input: PaymentInput) {
  return api.post<Payment>("/Payment", input);
}

export function collectPaymentBalance(paymentId: number, amount: number) {
  return api.post<Payment>(`/Payment/${paymentId}/collect`, { amount });
}
