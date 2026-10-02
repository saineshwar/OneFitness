import { api } from "@/lib/api";

export type User = {
  userId: number;
  userName: string;
  firstName: string;
  lastName: string | null;
  emailId: string;
  mobileNo: string | null;
  gender: string | null;
  roleId: number | null;
  roleName: string | null;
  status: boolean;
  isFirstLogin: boolean;
  createdOn: string;
  modifiedOn: string | null;
};

export type CreateUserInput = {
  userName: string;
  firstName: string;
  lastName: string | null;
  emailId: string;
  mobileNo: string | null;
  gender: string | null;
  roleId: number;
  password: string;
  confirmPassword: string;
};

export type UpdateUserInput = {
  firstName: string;
  lastName: string | null;
  emailId: string;
  mobileNo: string | null;
  gender: string | null;
  roleId: number;
  status: boolean;
};

export function getUsers() {
  return api.get<User[]>("/User");
}

export function createUser(input: CreateUserInput) {
  return api.post<User>("/User", input);
}

export function updateUser(userId: number, input: UpdateUserInput) {
  return api.put<User>(`/User/${userId}`, input);
}

export function deleteUser(userId: number) {
  return api.delete(`/User/${userId}`);
}

export type ResetPasswordInput = {
  newPassword: string;
  confirmPassword: string;
};

export function resetUserPassword(userId: number, input: ResetPasswordInput) {
  return api.post<User>(`/User/${userId}/reset-password`, input);
}
