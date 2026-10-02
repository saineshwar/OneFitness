import { api, ApiError, type PagedResult } from "@/lib/api";

export type Member = {
  memberId: number;
  memberNo: string;
  firstName: string;
  lastName: string | null;
  middleName: string | null;
  dob: string | null;
  age: number | null;
  mobileNo: string | null;
  emailId: string | null;
  genderId: number | null;
  address: string;
  joiningDate: string | null;
  emergencyContactName: string | null;
  emergencyContactNo: string | null;
  status: boolean;
  createdOn: string;
  modifiedOn: string | null;
};

export type MemberInput = {
  firstName: string;
  lastName: string;
  middleName: string;
  dob: string;
  age: number;
  mobileNo: string;
  emailId: string;
  genderId: number;
  address: string;
  joiningDate: string;
  emergencyContactName: string;
  emergencyContactNo: string;
  status: boolean;
};

export function getMembers() {
  return api.get<Member[]>("/Member");
}

export function getMembersPaged(params: { page: number; pageSize: number; search?: string }) {
  const query = new URLSearchParams({
    page: String(params.page),
    pageSize: String(params.pageSize),
  });
  if (params.search) {
    query.set("search", params.search);
  }
  return api.get<PagedResult<Member>>(`/Member/paged?${query.toString()}`);
}

export function createMember(input: MemberInput) {
  return api.post<Member>("/Member", input);
}

export function updateMember(memberId: number, input: MemberInput) {
  return api.put<Member>(`/Member/${memberId}`, input);
}

export function deleteMember(memberId: number) {
  return api.delete(`/Member/${memberId}`);
}

/** Returns the member's photo as a data URL, or null when none has been taken. */
export async function getMemberPhoto(memberId: number): Promise<string | null> {
  try {
    const result = await api.get<{ photo: string }>(`/Member/${memberId}/photo`);
    return result.photo;
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return null;
    throw error;
  }
}

export function saveMemberPhoto(memberId: number, photo: string) {
  return api.put<{ photo: string }>(`/Member/${memberId}/photo`, { photo });
}

export function deleteMemberPhoto(memberId: number) {
  return api.delete(`/Member/${memberId}/photo`);
}
