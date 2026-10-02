import { api } from "@/lib/api";

export type Installment = {
  installmentId: number;
  installmentName: string;
  status: boolean;
  installmentMonths: number | null;
  createdOn: string;
  modifiedOn: string | null;
};

export type InstallmentInput = {
  installmentName: string;
  status: boolean;
  installmentMonths: number | null;
};

export function getInstallments() {
  return api.get<Installment[]>("/Installment");
}

export function createInstallment(input: InstallmentInput) {
  return api.post<Installment>("/Installment", input);
}

export function updateInstallment(installmentId: number, input: InstallmentInput) {
  return api.put<Installment>(`/Installment/${installmentId}`, input);
}

export function deleteInstallment(installmentId: number) {
  return api.delete(`/Installment/${installmentId}`);
}
