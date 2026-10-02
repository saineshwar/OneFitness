import { api } from "@/lib/api";

export type PaymentType = {
  paymentTypeId: number;
  paymentTypeName: string;
  status: boolean;
};

export function getPaymentTypes() {
  return api.get<PaymentType[]>("/PaymentType");
}
