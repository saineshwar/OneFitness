import { api } from "@/lib/api";

export type WorkOut = {
  workOutId: number;
  workOutName: string;
  description: string;
  status: boolean;
  createdOn: string;
  modifiedOn: string | null;
};

export type WorkOutInput = {
  workOutName: string;
  description: string;
  status: boolean;
};

export function getWorkOuts() {
  return api.get<WorkOut[]>("/WorkOut");
}

export function createWorkOut(input: WorkOutInput) {
  return api.post<WorkOut>("/WorkOut", input);
}

export function updateWorkOut(workOutId: number, input: WorkOutInput) {
  return api.put<WorkOut>(`/WorkOut/${workOutId}`, input);
}

export function deleteWorkOut(workOutId: number) {
  return api.delete(`/WorkOut/${workOutId}`);
}
