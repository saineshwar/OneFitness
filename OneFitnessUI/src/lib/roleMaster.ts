import { api } from "@/lib/api";

export type Role = {
  roleId: number;
  roleName: string;
  status: boolean;
  createdOn: string;
  modifiedOn: string | null;
};

export type RoleInput = {
  roleName: string;
  status: boolean;
};

export function getRoles() {
  return api.get<Role[]>("/RoleMaster");
}

export function getActiveRoles() {
  return api.get<Role[]>("/RoleMaster/active");
}

export function createRole(input: RoleInput) {
  return api.post<Role>("/RoleMaster", input);
}

export function updateRole(roleId: number, input: RoleInput) {
  return api.put<Role>(`/RoleMaster/${roleId}`, input);
}

export function deleteRole(roleId: number) {
  return api.delete(`/RoleMaster/${roleId}`);
}
