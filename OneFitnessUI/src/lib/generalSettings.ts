import { api } from "@/lib/api";

export type GeneralSettings = {
  companyId: number;
  name: string;
  supportEmailId: string;
  websiteTitle: string;
  websiteUrl: string | null;
  telephoneNo: string | null;
  mobileNo: string | null;
  status: boolean;
  logopath: string | null;
  logoFileName: string | null;
  address: string;
  createdOn: string;
  modifiedOn: string | null;
};

export type GeneralSettingsInput = {
  name: string;
  supportEmailId: string;
  websiteTitle: string;
  websiteUrl: string;
  telephoneNo: string;
  mobileNo: string;
  status: boolean;
  logopath: string;
  logoFileName: string;
  address: string;
};

export function getGeneralSettingsList() {
  return api.get<GeneralSettings[]>("/GeneralSettings");
}

export function createGeneralSettings(input: GeneralSettingsInput) {
  return api.post<GeneralSettings>("/GeneralSettings", input);
}

export function updateGeneralSettings(companyId: number, input: GeneralSettingsInput) {
  return api.put<GeneralSettings>(`/GeneralSettings/${companyId}`, input);
}

export function deleteGeneralSettings(companyId: number) {
  return api.delete(`/GeneralSettings/${companyId}`);
}
