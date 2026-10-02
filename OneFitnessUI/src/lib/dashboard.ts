import { api } from "@/lib/api";

export type YearwiseChart = {
  april: number;
  may: number;
  june: number;
  july: number;
  august: number;
  sept: number;
  oct: number;
  nov: number;
  dec: number;
  jan: number;
  feb: number;
  march: number;
};

export type TopMembershipType = {
  membershipTypeId: number;
  membershipTypeName: string;
  totalCount: number;
};

export type DashboardSummary = {
  totalMembers: number;
  activeMembers: number;
  monthlyRevenue: number;
  newRegistrationsThisMonth: number;
  newRegistrationsToday: number;
  renewedThisMonth: number;
  refundsThisMonth: number;
  enquiryCount: number;
  yearwiseNewChart: YearwiseChart;
  yearwiseRenewedChart: YearwiseChart;
  topMembershipTypes: TopMembershipType[];
};

export function getDashboardSummary() {
  return api.get<DashboardSummary>("/Report/dashboard-summary");
}
