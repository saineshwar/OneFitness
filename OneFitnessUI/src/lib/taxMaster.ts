import { api } from "@/lib/api";

export type TaxMaster = {
  taxId: number;
  taxType: string;
  taxRate: number;
  status: boolean;
  identificationNo: string | null;
  createdOn: string;
  modifiedOn: string | null;
};

export type TaxMasterInput = {
  taxType: string;
  taxRate: number;
  status: boolean;
  identificationNo: string | null;
};

export function getTaxMasters() {
  return api.get<TaxMaster[]>("/TaxMaster");
}

export function createTaxMaster(input: TaxMasterInput) {
  return api.post<TaxMaster>("/TaxMaster", input);
}

export function updateTaxMaster(taxId: number, input: TaxMasterInput) {
  return api.put<TaxMaster>(`/TaxMaster/${taxId}`, input);
}

export function deleteTaxMaster(taxId: number) {
  return api.delete(`/TaxMaster/${taxId}`);
}
