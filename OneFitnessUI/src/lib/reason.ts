import { api } from "@/lib/api";

export type Reason = {
  reasonId: number;
  reasonName: string;
};

export type ReasonInput = {
  reasonName: string;
};

export function getReasons() {
  return api.get<Reason[]>("/Reason");
}

export function createReason(input: ReasonInput) {
  return api.post<Reason>("/Reason", input);
}

export function updateReason(reasonId: number, input: ReasonInput) {
  return api.put<Reason>(`/Reason/${reasonId}`, input);
}

export function deleteReason(reasonId: number) {
  return api.delete(`/Reason/${reasonId}`);
}
