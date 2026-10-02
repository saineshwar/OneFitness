import { api } from "@/lib/api";

export type Enquiry = {
  enquiryId: number;
  workOutId: number | null;
  firstName: string;
  lastName: string | null;
  middleName: string | null;
  mobileNo: string | null;
  emailId: string | null;
  genderId: number | null;
  reasonId: number | null;
  enquiryDetails: string | null;
  status: boolean;
  createdOn: string;
  modifiedOn: string | null;
};

export type EnquiryInput = {
  workOutId: number | null;
  firstName: string;
  lastName: string | null;
  middleName: string | null;
  mobileNo: string | null;
  emailId: string | null;
  genderId: number;
  reasonId: number | null;
  enquiryDetails: string | null;
  status: boolean;
};

export function getEnquiries() {
  return api.get<Enquiry[]>("/Enquiry");
}

export function createEnquiry(input: EnquiryInput) {
  return api.post<Enquiry>("/Enquiry", input);
}

export function updateEnquiry(enquiryId: number, input: EnquiryInput) {
  return api.put<Enquiry>(`/Enquiry/${enquiryId}`, input);
}

export function deleteEnquiry(enquiryId: number) {
  return api.delete(`/Enquiry/${enquiryId}`);
}
