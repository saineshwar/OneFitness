import { api } from "@/lib/api";

export type MembershipType = {
  membershipTypeId: number;
  membershipTypeName: string;
  amount: number;
  installmentId: number | null;
  workOutId: number | null;
  status: boolean;
  createdOn: string;
  modifiedOn: string | null;
};

export type MembershipTypeInput = {
  membershipTypeName: string;
  amount: number;
  installmentId: number;
  workOutId: number;
  status: boolean;
};

export function getMembershipTypes() {
  return api.get<MembershipType[]>("/MembershipType");
}

export function createMembershipType(input: MembershipTypeInput) {
  return api.post<MembershipType>("/MembershipType", input);
}

export function updateMembershipType(membershipTypeId: number, input: MembershipTypeInput) {
  return api.put<MembershipType>(`/MembershipType/${membershipTypeId}`, input);
}

export function deleteMembershipType(membershipTypeId: number) {
  return api.delete(`/MembershipType/${membershipTypeId}`);
}
