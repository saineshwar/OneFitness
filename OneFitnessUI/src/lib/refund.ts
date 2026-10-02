import { api, type PagedResult } from "@/lib/api";

export type Refund = {
  refundId: number;
  memberId: number;
  memberNo: string;
  memberFullName: string;
  amount: number;
  status: boolean;
  createdOn: string;
  modifiedOn: string | null;
};

export type RefundInput = {
  memberId: number;
  amount: number;
};

export function getRefunds() {
  return api.get<Refund[]>("/Refund");
}

export function getRefundsPaged(params: { page: number; pageSize: number; search?: string }) {
  const query = new URLSearchParams({
    page: String(params.page),
    pageSize: String(params.pageSize),
  });
  if (params.search) {
    query.set("search", params.search);
  }
  return api.get<PagedResult<Refund>>(`/Refund/paged?${query.toString()}`);
}

export function createRefund(input: RefundInput) {
  return api.post<Refund>("/Refund", input);
}

export function deleteRefund(refundId: number) {
  return api.delete(`/Refund/${refundId}`);
}
