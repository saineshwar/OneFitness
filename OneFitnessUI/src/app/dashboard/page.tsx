"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { Icon } from "@/components/icons";
import { StatusBadge } from "@/components/StatusBadge";
import { getDashboardSummary, type DashboardSummary, type TopMembershipType, type YearwiseChart } from "@/lib/dashboard";
import { getMembersPaged, type Member } from "@/lib/member";
import { getLatestPaymentByMember, type Payment } from "@/lib/payment";
import { getMembershipTypes, type MembershipType } from "@/lib/membershipType";
import { ApiError } from "@/lib/api";

const currency = new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR", maximumFractionDigits: 0 });

function formatDate(value: string | null) {
  if (!value) return "—";
  return new Date(value).toLocaleDateString("en-IN", { year: "numeric", month: "short", day: "numeric" });
}

const MONTHS: { key: keyof YearwiseChart; label: string }[] = [
  { key: "april", label: "Apr" },
  { key: "may", label: "May" },
  { key: "june", label: "Jun" },
  { key: "july", label: "Jul" },
  { key: "august", label: "Aug" },
  { key: "sept", label: "Sep" },
  { key: "oct", label: "Oct" },
  { key: "nov", label: "Nov" },
  { key: "dec", label: "Dec" },
  { key: "jan", label: "Jan" },
  { key: "feb", label: "Feb" },
  { key: "march", label: "Mar" },
];

export default function DashboardPage() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [summaryError, setSummaryError] = useState<string | null>(null);
  const [recentMembers, setRecentMembers] = useState<Member[] | null>(null);
  const [recentPayments, setRecentPayments] = useState<Record<number, Payment | null>>({});
  const [membershipTypes, setMembershipTypes] = useState<MembershipType[]>([]);

  function loadSummary() {
    setSummaryError(null);
    getDashboardSummary()
      .then(setSummary)
      .catch((error) => setSummaryError(error instanceof ApiError ? error.message : "Failed to load dashboard data."));
  }

  useEffect(() => {
    loadSummary();
    getMembersPaged({ page: 1, pageSize: 5 })
      .then(async (result) => {
        setRecentMembers(result.items);
        const entries = await Promise.all(
          result.items.map(async (member) => {
            try {
              return [member.memberId, await getLatestPaymentByMember(member.memberId)] as const;
            } catch {
              return [member.memberId, null] as const;
            }
          }),
        );
        setRecentPayments(Object.fromEntries(entries));
      })
      .catch(() => setRecentMembers([]));
    getMembershipTypes().then(setMembershipTypes).catch(() => setMembershipTypes([]));
  }, []);

  function membershipTypeName(id: number | null | undefined) {
    return membershipTypes.find((m) => m.membershipTypeId === id)?.membershipTypeName ?? "—";
  }

  const stats = summary
    ? [
        {
          label: "Total Members",
          value: summary.totalMembers.toLocaleString(),
          icon: "members" as const,
          accent: "bg-indigo-50 text-indigo-600 dark:bg-indigo-500/10 dark:text-indigo-300",
        },
        {
          label: "Active Memberships",
          value: summary.activeMembers.toLocaleString(),
          icon: "shield" as const,
          accent: "bg-emerald-50 text-emerald-600 dark:bg-emerald-500/10 dark:text-emerald-300",
        },
        {
          label: "Revenue This Month",
          value: currency.format(summary.monthlyRevenue),
          icon: "chart" as const,
          accent: "bg-purple-50 text-purple-600 dark:bg-purple-500/10 dark:text-purple-300",
        },
        {
          label: "New This Month",
          value: summary.newRegistrationsThisMonth.toLocaleString(),
          icon: "trendUp" as const,
          accent: "bg-amber-50 text-amber-600 dark:bg-amber-500/10 dark:text-amber-300",
        },
      ]
    : [];

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Dashboard</h1>
        <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">Overview of your fitness center.</p>
      </div>

      {summaryError && (
        <div className="flex items-center justify-between gap-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300">
          <span>{summaryError}</span>
          <button
            type="button"
            onClick={loadSummary}
            className="shrink-0 rounded-md bg-white px-2.5 py-1 text-xs font-medium text-red-700 shadow-sm transition hover:bg-red-100 dark:bg-slate-900 dark:hover:bg-slate-800"
          >
            Retry
          </button>
        </div>
      )}

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 xl:grid-cols-4">
        {summary === null &&
          !summaryError &&
          [0, 1, 2, 3].map((i) => (
            <div
              key={i}
              className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900"
            >
              <div className="h-11 w-11 animate-pulse rounded-lg bg-slate-100 dark:bg-slate-800" />
              <div className="mt-4 h-7 w-20 animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
              <div className="mt-2 h-4 w-28 animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
            </div>
          ))}

        {stats.map((stat) => (
          <div
            key={stat.label}
            className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm transition hover:shadow-md dark:border-slate-800 dark:bg-slate-900"
          >
            <div className={`flex h-11 w-11 items-center justify-center rounded-lg ${stat.accent}`}>
              <Icon name={stat.icon} className="h-5 w-5" />
            </div>
            <p className="mt-4 text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">{stat.value}</p>
            <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">{stat.label}</p>
          </div>
        ))}
      </div>

      {summary && (
        <div className="grid grid-cols-2 gap-4 sm:grid-cols-4">
          <MiniStat label="New Today" value={summary.newRegistrationsToday} />
          <MiniStat label="Renewed This Month" value={summary.renewedThisMonth} />
          <MiniStat label="Refunds This Month" value={summary.refundsThisMonth} />
          <MiniStat label="Total Enquiries" value={summary.enquiryCount} />
        </div>
      )}

      <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
        <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm lg:col-span-2 dark:border-slate-800 dark:bg-slate-900">
          <h2 className="mb-1 text-base font-semibold tracking-tight text-slate-900 dark:text-white">New vs Renewed</h2>
          <p className="mb-4 text-xs text-slate-400">Fiscal year to date (Apr–Mar)</p>
          {summary ? (
            <NewVsRenewedChart newChart={summary.yearwiseNewChart} renewedChart={summary.yearwiseRenewedChart} />
          ) : (
            <div className="h-40 animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
          )}
        </div>

        <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
          <h2 className="mb-4 text-base font-semibold tracking-tight text-slate-900 dark:text-white">Top Membership Types</h2>
          {summary ? (
            <TopPlansList items={summary.topMembershipTypes} />
          ) : (
            <div className="h-40 animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
          )}
        </div>
      </div>

      <div className="rounded-xl border border-slate-200/70 bg-white shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className="flex items-center justify-between border-b border-slate-100 px-5 py-4 dark:border-slate-800">
          <h2 className="text-base font-semibold tracking-tight text-slate-900 dark:text-white">Recent Members</h2>
          <Link
            href="/dashboard/members"
            className="text-sm font-medium text-indigo-600 transition hover:text-indigo-500 dark:text-indigo-400 dark:hover:text-indigo-300"
          >
            View all
          </Link>
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
                <th className="px-5 py-3 font-medium">Name</th>
                <th className="px-5 py-3 font-medium">Plan</th>
                <th className="px-5 py-3 font-medium">Joined</th>
                <th className="px-5 py-3 font-medium">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {recentMembers === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={4}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {recentMembers !== null && recentMembers.length === 0 && (
                <tr>
                  <td colSpan={4} className="px-5 py-10 text-center text-sm text-slate-400">
                    No members yet.
                  </td>
                </tr>
              )}

              {recentMembers?.map((member) => (
                <tr key={member.memberId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">
                    {[member.firstName, member.middleName, member.lastName].filter(Boolean).join(" ")}
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                    {membershipTypeName(recentPayments[member.memberId]?.membershipTypeId)}
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(member.joiningDate)}</td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={member.status} />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

function MiniStat({ label, value }: { label: string; value: number }) {
  return (
    <div className="rounded-xl border border-slate-200/70 bg-white px-4 py-3 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <p className="text-lg font-semibold text-slate-900 dark:text-white">{value.toLocaleString()}</p>
      <p className="text-xs text-slate-500 dark:text-slate-400">{label}</p>
    </div>
  );
}

function NewVsRenewedChart({ newChart, renewedChart }: { newChart: YearwiseChart; renewedChart: YearwiseChart }) {
  const maxValue = Math.max(1, ...MONTHS.flatMap((m) => [newChart[m.key], renewedChart[m.key]]));
  const plotHeight = 140;

  return (
    <div>
      <div className="mb-4 flex items-center gap-4 text-xs text-slate-500 dark:text-slate-400">
        <span className="flex items-center gap-1.5">
          <span className="h-2.5 w-2.5 rounded-full bg-[#2a78d6] dark:bg-[#3987e5]" />
          New members
        </span>
        <span className="flex items-center gap-1.5">
          <span className="h-2.5 w-2.5 rounded-full bg-[#eb6834] dark:bg-[#d95926]" />
          Renewed
        </span>
      </div>
      <div className="flex items-end gap-2 border-b border-slate-100 dark:border-slate-800" style={{ height: plotHeight }}>
        {MONTHS.map((m) => {
          const newValue = newChart[m.key];
          const renewedValue = renewedChart[m.key];
          return (
            <div key={m.key} className="flex flex-1 flex-col items-center justify-end gap-1">
              <div className="flex items-end gap-0.5">
                <div
                  className="w-2.5 rounded-t bg-[#2a78d6] dark:bg-[#3987e5]"
                  style={{ height: `${(newValue / maxValue) * plotHeight}px` }}
                  title={`${m.label}: ${newValue} new`}
                />
                <div
                  className="w-2.5 rounded-t bg-[#eb6834] dark:bg-[#d95926]"
                  style={{ height: `${(renewedValue / maxValue) * plotHeight}px` }}
                  title={`${m.label}: ${renewedValue} renewed`}
                />
              </div>
            </div>
          );
        })}
      </div>
      <div className="mt-1.5 flex gap-2">
        {MONTHS.map((m) => (
          <span key={m.key} className="flex-1 text-center text-[10px] text-slate-400">
            {m.label}
          </span>
        ))}
      </div>
    </div>
  );
}

function TopPlansList({ items }: { items: TopMembershipType[] }) {
  if (items.length === 0) {
    return <p className="text-sm text-slate-400">No membership data yet this fiscal year.</p>;
  }

  const maxCount = Math.max(1, ...items.map((i) => i.totalCount));

  return (
    <div className="space-y-3">
      {items.map((item) => (
        <div key={item.membershipTypeId}>
          <div className="mb-1 flex items-center justify-between text-sm">
            <span className="text-slate-700 dark:text-slate-300">{item.membershipTypeName}</span>
            <span className="font-medium text-slate-900 dark:text-white">{item.totalCount}</span>
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-slate-100 dark:bg-slate-800">
            <div
              className="h-full rounded-full bg-[#2a78d6] dark:bg-[#3987e5]"
              style={{ width: `${(item.totalCount / maxCount) * 100}%` }}
            />
          </div>
        </div>
      ))}
    </div>
  );
}
