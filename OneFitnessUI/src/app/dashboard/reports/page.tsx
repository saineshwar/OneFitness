"use client";

import { useEffect, useState } from "react";
import { Field, fieldInputClass, inputBorderClass } from "@/components/Field";
import { StatusBadge } from "@/components/StatusBadge";
import { ApiError } from "@/lib/api";
import {
  getYearwiseReport,
  getMonthwiseReport,
  getRenewalReport,
  getJoinedReport,
  getRefundReport,
  getIncomeCreditDebitReport,
  downloadTallyExport,
  getTaxSummaryReport,
  getPaymentTypeCollectionReport,
  getStaffCollectionReport,
  getRenewalStatusReport,
  getOutstandingBalancesReport,
  type YearwiseReportRow,
  type MonthlyReportMember,
  type RenewalReportRow,
  type MemberJoinedRow,
  type RefundReportRow,
  type IncomeCreditDebitRow,
  type TaxSummaryRow,
  type PaymentTypeCollectionRow,
  type StaffCollectionRow,
  type RenewalStatusRow,
  type OutstandingBalanceRow,
} from "@/lib/report";
import { downloadCsv } from "@/lib/csv";

const currency = new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR", maximumFractionDigits: 0 });

function formatDate(value: string | null) {
  if (!value) return "—";
  return new Date(value).toLocaleDateString("en-IN", { year: "numeric", month: "short", day: "numeric" });
}

function toDateInputValue(date: Date) {
  return date.toISOString().slice(0, 10);
}

function defaultFromDate() {
  const date = new Date();
  date.setDate(1);
  return toDateInputValue(date);
}

const MONTHS = [
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
] as const satisfies readonly { key: keyof YearwiseReportRow; label: string }[];

const MONTH_NAMES = [
  "January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December",
];

type TabKey =
  | "yearwise"
  | "monthwise"
  | "joined"
  | "renewal"
  | "refund"
  | "credit-debit"
  | "tax-summary"
  | "payment-type"
  | "staff-collection"
  | "renewal-status"
  | "outstanding-balances";

const tabs: { key: TabKey; label: string }[] = [
  { key: "yearwise", label: "Yearwise" },
  { key: "monthwise", label: "Monthwise" },
  { key: "joined", label: "Members Joined" },
  { key: "renewal", label: "Renewal" },
  { key: "refund", label: "Refund" },
  { key: "credit-debit", label: "Credit/Debit" },
  { key: "tax-summary", label: "Tax Summary" },
  { key: "payment-type", label: "Payment Type" },
  { key: "staff-collection", label: "Staff Collection" },
  { key: "renewal-status", label: "Renewals Due / Lapsed" },
  { key: "outstanding-balances", label: "Outstanding Balances" },
];

export default function ReportsPage() {
  const [tab, setTab] = useState<TabKey>("yearwise");

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Reports</h1>
        <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
          Generate yearwise, monthwise, renewal and refund reports.
        </p>
      </div>

      <div className="flex w-fit flex-wrap gap-1 rounded-lg border border-slate-200/70 bg-white p-1 dark:border-slate-800 dark:bg-slate-900">
        {tabs.map((t) => (
          <button
            key={t.key}
            type="button"
            onClick={() => setTab(t.key)}
            className={`rounded-md px-4 py-2 text-sm font-medium transition ${
              tab === t.key
                ? "bg-indigo-600 text-white shadow-sm shadow-indigo-600/20"
                : "text-slate-600 hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-800"
            }`}
          >
            {t.label}
          </button>
        ))}
      </div>

      {tab === "yearwise" && <YearwiseTab />}
      {tab === "monthwise" && <MonthwiseTab />}
      {tab === "joined" && <JoinedTab />}
      {tab === "renewal" && <RenewalTab />}
      {tab === "refund" && <RefundTab />}
      {tab === "credit-debit" && <CreditDebitTab />}
      {tab === "tax-summary" && <TaxSummaryTab />}
      {tab === "payment-type" && <PaymentTypeTab />}
      {tab === "staff-collection" && <StaffCollectionTab />}
      {tab === "renewal-status" && <RenewalStatusTab />}
      {tab === "outstanding-balances" && <OutstandingBalancesTab />}
    </div>
  );
}

function ErrorBanner({ message, onRetry }: { message: string; onRetry: () => void }) {
  return (
    <div className="flex items-center justify-between gap-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300">
      <span>{message}</span>
      <button
        type="button"
        onClick={onRetry}
        className="shrink-0 rounded-md bg-white px-2.5 py-1 text-xs font-medium text-red-700 shadow-sm transition hover:bg-red-100 dark:bg-slate-900 dark:hover:bg-slate-800"
      >
        Retry
      </button>
    </div>
  );
}

function GenerateButton({ loading, onClick }: { loading: boolean; onClick: () => void }) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={loading}
      className="flex items-center gap-2 rounded-lg bg-indigo-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500 disabled:cursor-not-allowed disabled:opacity-60"
    >
      {loading && <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />}
      {loading ? "Loading…" : "Generate"}
    </button>
  );
}

function ExportCsvButton({ onClick, disabled }: { onClick: () => void; disabled: boolean }) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      className="rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
    >
      Export CSV
    </button>
  );
}

function TableSkeleton({ colSpan }: { colSpan: number }) {
  return (
    <>
      {[0, 1, 2].map((i) => (
        <tr key={i}>
          <td className="px-5 py-4" colSpan={colSpan}>
            <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
          </td>
        </tr>
      ))}
    </>
  );
}

function EmptyRow({ colSpan, message }: { colSpan: number; message: string }) {
  return (
    <tr>
      <td colSpan={colSpan} className="px-5 py-10 text-center text-sm text-slate-400">
        {message}
      </td>
    </tr>
  );
}

function SummaryItem({ label, value, bold }: { label: string; value: string; bold?: boolean }) {
  return (
    <div className="rounded-xl border border-slate-200/70 bg-white p-4 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <p className="text-[11px] text-slate-400">{label}</p>
      <p className={`text-slate-800 dark:text-slate-200 ${bold ? "font-semibold" : ""}`}>{value}</p>
    </div>
  );
}

function ReportCard({ children }: { children: React.ReactNode }) {
  return (
    <div className="rounded-xl border border-slate-200/70 bg-white shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="overflow-x-auto">
        <table className="w-full text-left text-sm">{children}</table>
      </div>
    </div>
  );
}

function YearwiseTab() {
  const [fiscalYear, setFiscalYear] = useState(new Date().getFullYear());
  const [row, setRow] = useState<YearwiseReportRow | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [searched, setSearched] = useState(false);

  async function runReport() {
    setLoading(true);
    setError(null);
    setSearched(true);
    try {
      setRow(await getYearwiseReport(fiscalYear));
    } catch (err) {
      setRow(null);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function exportCsv() {
    if (!row) return;
    downloadCsv(
      `yearwise_${fiscalYear}.csv`,
      ["Fiscal Year", ...MONTHS.map((m) => m.label), "Total"],
      [[row.fiscalYear, ...MONTHS.map((m) => row[m.key]), row.total]],
    );
  }

  const colSpan = MONTHS.length + 2;

  return (
    <div className="space-y-4">
      <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-wrap items-end gap-4">
          <Field label="Fiscal Year" className="w-40">
            <input
              type="number"
              value={fiscalYear}
              onChange={(e) => setFiscalYear(Number(e.target.value))}
              className={`${fieldInputClass} ${inputBorderClass(false)}`}
            />
          </Field>
          <GenerateButton loading={loading} onClick={runReport} />
          <ExportCsvButton onClick={exportCsv} disabled={!row} />
        </div>
      </div>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-4 py-3 font-medium">Fiscal Year</th>
            {MONTHS.map((m) => (
              <th key={m.key} className="px-4 py-3 font-medium">
                {m.label}
              </th>
            ))}
            <th className="px-4 py-3 font-medium">Total</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={colSpan} />}
          {!loading && !row && searched && !error && (
            <EmptyRow colSpan={colSpan} message="No data for the selected fiscal year." />
          )}
          {!loading && row && (
            <tr className="hover:bg-slate-50 dark:hover:bg-slate-800/50">
              <td className="px-4 py-3.5 font-medium text-slate-800 dark:text-slate-200">{row.fiscalYear}</td>
              {MONTHS.map((m) => (
                <td key={m.key} className="px-4 py-3.5 text-slate-500 dark:text-slate-400">
                  {row[m.key]}
                </td>
              ))}
              <td className="px-4 py-3.5 font-semibold text-slate-800 dark:text-slate-200">{row.total}</td>
            </tr>
          )}
        </tbody>
      </ReportCard>
    </div>
  );
}

function MonthwiseTab() {
  const now = new Date();
  const [year, setYear] = useState(now.getFullYear());
  const [month, setMonth] = useState(now.getMonth() + 1);
  const [members, setMembers] = useState<MonthlyReportMember[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setMembers(await getMonthwiseReport(year, month));
    } catch (err) {
      setMembers([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function exportCsv() {
    if (!members || members.length === 0) return;
    downloadCsv(
      `monthwise_${year}-${String(month).padStart(2, "0")}.csv`,
      ["Member No", "Name", "Joined", "Total Amount", "Status"],
      members.map((m) => [
        m.memberNo,
        [m.firstName, m.middleName, m.lastName].filter(Boolean).join(" "),
        formatDate(m.createdOn),
        m.totalAmount ?? "",
        m.status ? "Active" : "Inactive",
      ]),
    );
  }

  return (
    <div className="space-y-4">
      <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-wrap items-end gap-4">
          <Field label="Year" className="w-32">
            <input
              type="number"
              value={year}
              onChange={(e) => setYear(Number(e.target.value))}
              className={`${fieldInputClass} ${inputBorderClass(false)}`}
            />
          </Field>
          <Field label="Month" className="w-44">
            <select
              value={month}
              onChange={(e) => setMonth(Number(e.target.value))}
              className={`${fieldInputClass} ${inputBorderClass(false)}`}
            >
              {MONTH_NAMES.map((name, idx) => (
                <option key={name} value={idx + 1}>
                  {name}
                </option>
              ))}
            </select>
          </Field>
          <GenerateButton loading={loading} onClick={runReport} />
          <ExportCsvButton onClick={exportCsv} disabled={!members || members.length === 0} />
        </div>
      </div>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Member No</th>
            <th className="px-5 py-3 font-medium">Name</th>
            <th className="px-5 py-3 font-medium">Joined</th>
            <th className="px-5 py-3 font-medium">Total Amount</th>
            <th className="px-5 py-3 font-medium">Status</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={5} />}
          {!loading && members !== null && members.length === 0 && !error && (
            <EmptyRow colSpan={5} message="No members found for the selected month." />
          )}
          {!loading &&
            members?.map((m) => (
              <tr key={m.memberId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{m.memberNo}</td>
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">
                  {[m.firstName, m.middleName, m.lastName].filter(Boolean).join(" ")}
                </td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(m.createdOn)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                  {m.totalAmount != null ? currency.format(m.totalAmount) : "—"}
                </td>
                <td className="px-5 py-3.5">
                  <StatusBadge active={m.status} />
                </td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function DateRangeFilter({
  fromDate,
  toDate,
  onFromDateChange,
  onToDateChange,
  loading,
  onGenerate,
  children,
}: {
  fromDate: string;
  toDate: string;
  onFromDateChange: (value: string) => void;
  onToDateChange: (value: string) => void;
  loading: boolean;
  onGenerate: () => void;
  children?: React.ReactNode;
}) {
  return (
    <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
      <div className="flex flex-wrap items-end gap-4">
        <Field label="From Date" className="w-44">
          <input
            type="date"
            value={fromDate}
            onChange={(e) => onFromDateChange(e.target.value)}
            className={`${fieldInputClass} ${inputBorderClass(false)}`}
          />
        </Field>
        <Field label="To Date" className="w-44">
          <input
            type="date"
            value={toDate}
            onChange={(e) => onToDateChange(e.target.value)}
            className={`${fieldInputClass} ${inputBorderClass(false)}`}
          />
        </Field>
        <GenerateButton loading={loading} onClick={onGenerate} />
        {children}
      </div>
    </div>
  );
}

function JoinedTab() {
  const [fromDate, setFromDate] = useState(defaultFromDate());
  const [toDate, setToDate] = useState(toDateInputValue(new Date()));
  const [rows, setRows] = useState<MemberJoinedRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getJoinedReport({ fromDate, toDate }));
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      `members-joined_${fromDate}_to_${toDate}.csv`,
      ["Member No", "Name", "Workout", "Membership Type", "Installment", "Joining Date", "Amount", "Mobile", "Email", "Status"],
      rows.map((r) => [
        r.memberNo,
        r.fullName,
        r.workOutName ?? "",
        r.membershipTypeName ?? "",
        r.installmentName ?? "",
        formatDate(r.joiningDate),
        r.totalAmount ?? "",
        r.mobileNo,
        r.emailId,
        r.status ? "Active" : "Inactive",
      ]),
    );
  }

  return (
    <div className="space-y-4">
      <DateRangeFilter
        fromDate={fromDate}
        toDate={toDate}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        loading={loading}
        onGenerate={runReport}
      >
        <ExportCsvButton onClick={exportCsv} disabled={!rows || rows.length === 0} />
      </DateRangeFilter>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <SummaryItem label="Members Joined" value={(rows?.length ?? 0).toString()} bold />

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Member No</th>
            <th className="px-5 py-3 font-medium">Name</th>
            <th className="px-5 py-3 font-medium">Workout</th>
            <th className="px-5 py-3 font-medium">Membership Type</th>
            <th className="px-5 py-3 font-medium">Joining Date</th>
            <th className="px-5 py-3 font-medium">Amount</th>
            <th className="px-5 py-3 font-medium">Mobile</th>
            <th className="px-5 py-3 font-medium">Status</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={8} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={8} message="No members joined in the selected range." />
          )}
          {!loading &&
            rows?.map((r) => (
              <tr key={r.memberId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{r.memberNo}</td>
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.fullName}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.workOutName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.membershipTypeName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(r.joiningDate)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                  {r.totalAmount != null ? currency.format(r.totalAmount) : "—"}
                </td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.mobileNo}</td>
                <td className="px-5 py-3.5">
                  <StatusBadge active={r.status} />
                </td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function RenewalTab() {
  const [fromDate, setFromDate] = useState(defaultFromDate());
  const [toDate, setToDate] = useState(toDateInputValue(new Date()));
  const [rows, setRows] = useState<RenewalReportRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getRenewalReport({ fromDate, toDate }));
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      `renewal_${fromDate}_to_${toDate}.csv`,
      ["Member No", "Name", "Workout", "Membership Type", "Installment", "Joining Date", "Next Renewal", "Amount", "Mobile"],
      rows.map((r) => [
        r.memberNo,
        r.fullName,
        r.workOutName ?? "",
        r.membershipTypeName ?? "",
        r.installmentName ?? "",
        formatDate(r.joiningDate),
        formatDate(r.nextRenewalDate),
        r.totalAmount ?? "",
        r.mobileNo,
      ]),
    );
  }

  return (
    <div className="space-y-4">
      <DateRangeFilter
        fromDate={fromDate}
        toDate={toDate}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        loading={loading}
        onGenerate={runReport}
      >
        <ExportCsvButton onClick={exportCsv} disabled={!rows || rows.length === 0} />
      </DateRangeFilter>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Member No</th>
            <th className="px-5 py-3 font-medium">Name</th>
            <th className="px-5 py-3 font-medium">Workout</th>
            <th className="px-5 py-3 font-medium">Membership Type</th>
            <th className="px-5 py-3 font-medium">Installment</th>
            <th className="px-5 py-3 font-medium">Joining Date</th>
            <th className="px-5 py-3 font-medium">Next Renewal</th>
            <th className="px-5 py-3 font-medium">Amount</th>
            <th className="px-5 py-3 font-medium">Mobile</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={9} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={9} message="No renewals found in the selected range." />
          )}
          {!loading &&
            rows?.map((r) => (
              <tr key={r.memberId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{r.memberNo}</td>
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.fullName}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.workOutName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.membershipTypeName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.installmentName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(r.joiningDate)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(r.nextRenewalDate)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                  {r.totalAmount != null ? currency.format(r.totalAmount) : "—"}
                </td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.mobileNo}</td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function RefundTab() {
  const [fromDate, setFromDate] = useState(defaultFromDate());
  const [toDate, setToDate] = useState(toDateInputValue(new Date()));
  const [rows, setRows] = useState<RefundReportRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getRefundReport({ fromDate, toDate }));
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      `refund_${fromDate}_to_${toDate}.csv`,
      ["Member No", "Name", "Workout", "Membership Type", "Subscription", "Refund Amount", "Refunded On", "Mobile"],
      rows.map((r) => [
        r.memberNo,
        r.fullName,
        r.workOutName ?? "",
        r.membershipTypeName ?? "",
        r.subscriptionAmount ?? "",
        r.refundAmount,
        formatDate(r.refundedDate),
        r.mobileNo,
      ]),
    );
  }

  return (
    <div className="space-y-4">
      <DateRangeFilter
        fromDate={fromDate}
        toDate={toDate}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        loading={loading}
        onGenerate={runReport}
      >
        <ExportCsvButton onClick={exportCsv} disabled={!rows || rows.length === 0} />
      </DateRangeFilter>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Member No</th>
            <th className="px-5 py-3 font-medium">Name</th>
            <th className="px-5 py-3 font-medium">Workout</th>
            <th className="px-5 py-3 font-medium">Membership Type</th>
            <th className="px-5 py-3 font-medium">Subscription</th>
            <th className="px-5 py-3 font-medium">Refund Amount</th>
            <th className="px-5 py-3 font-medium">Refunded On</th>
            <th className="px-5 py-3 font-medium">Mobile</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={8} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={8} message="No refunds found in the selected range." />
          )}
          {!loading &&
            rows?.map((r) => (
              <tr key={r.refundId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{r.memberNo}</td>
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.fullName}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.workOutName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.membershipTypeName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                  {r.subscriptionAmount != null ? currency.format(r.subscriptionAmount) : "—"}
                </td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{currency.format(r.refundAmount)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(r.refundedDate)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.mobileNo}</td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function CreditDebitTab() {
  const [fromDate, setFromDate] = useState(defaultFromDate());
  const [toDate, setToDate] = useState(toDateInputValue(new Date()));
  const [rows, setRows] = useState<IncomeCreditDebitRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [tallyExporting, setTallyExporting] = useState(false);
  const [tallyError, setTallyError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getIncomeCreditDebitReport({ fromDate, toDate }));
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function exportTally() {
    setTallyExporting(true);
    setTallyError(null);
    try {
      await downloadTallyExport({ fromDate, toDate });
    } catch (err) {
      setTallyError(err instanceof ApiError ? err.message : "Failed to generate Tally export.");
    } finally {
      setTallyExporting(false);
    }
  }

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      `credit-debit_${fromDate}_to_${toDate}.csv`,
      ["Date", "Voucher Type", "Particulars", "Member No", "Invoice No", "Credit", "Debit"],
      rows.map((r) => [
        formatDate(r.date),
        r.voucherType,
        r.particulars,
        r.memberNo,
        r.invoiceNo ?? "",
        r.creditAmount.toFixed(2),
        r.debitAmount.toFixed(2),
      ]),
    );
  }

  const totalCredit = rows?.reduce((sum, r) => sum + r.creditAmount, 0) ?? 0;
  const totalDebit = rows?.reduce((sum, r) => sum + r.debitAmount, 0) ?? 0;

  return (
    <div className="space-y-4">
      <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-wrap items-end gap-4">
          <Field label="From Date" className="w-44">
            <input
              type="date"
              value={fromDate}
              onChange={(e) => setFromDate(e.target.value)}
              className={`${fieldInputClass} ${inputBorderClass(false)}`}
            />
          </Field>
          <Field label="To Date" className="w-44">
            <input
              type="date"
              value={toDate}
              onChange={(e) => setToDate(e.target.value)}
              className={`${fieldInputClass} ${inputBorderClass(false)}`}
            />
          </Field>
          <GenerateButton loading={loading} onClick={runReport} />
          <button
            type="button"
            onClick={exportCsv}
            disabled={!rows || rows.length === 0}
            className="rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
          >
            Export CSV
          </button>
          <button
            type="button"
            onClick={exportTally}
            disabled={!rows || rows.length === 0 || tallyExporting}
            className="flex items-center gap-2 rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-medium text-slate-700 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
          >
            {tallyExporting && (
              <span className="h-4 w-4 animate-spin rounded-full border-2 border-slate-400/40 border-t-slate-600 dark:border-t-slate-200" />
            )}
            {tallyExporting ? "Generating…" : "Export for Tally"}
          </button>
        </div>
      </div>

      {error && <ErrorBanner message={error} onRetry={runReport} />}
      {tallyError && <ErrorBanner message={tallyError} onRetry={exportTally} />}

      <p className="text-xs text-slate-400">
        Tally export posts Cash, Membership Income and Output GST ledger lines (configured in the API). Test-import
        into a sample Tally company before using with live books.
      </p>

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
        <SummaryItem label="Total Credit" value={currency.format(totalCredit)} bold />
        <SummaryItem label="Total Debit" value={currency.format(totalDebit)} bold />
        <SummaryItem label="Net" value={currency.format(totalCredit - totalDebit)} bold />
      </div>

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Date</th>
            <th className="px-5 py-3 font-medium">Voucher Type</th>
            <th className="px-5 py-3 font-medium">Particulars</th>
            <th className="px-5 py-3 font-medium">Member No</th>
            <th className="px-5 py-3 font-medium">Invoice No</th>
            <th className="px-5 py-3 font-medium">Credit</th>
            <th className="px-5 py-3 font-medium">Debit</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={7} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={7} message="No transactions found in the selected range." />
          )}
          {!loading &&
            rows?.map((r, idx) => (
              <tr key={idx} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(r.date)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.voucherType}</td>
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.particulars}</td>
                <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{r.memberNo}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.invoiceNo ?? "—"}</td>
                <td className="px-5 py-3.5 text-emerald-600 dark:text-emerald-400">
                  {r.creditAmount > 0 ? currency.format(r.creditAmount) : "—"}
                </td>
                <td className="px-5 py-3.5 text-red-600 dark:text-red-400">
                  {r.debitAmount > 0 ? currency.format(r.debitAmount) : "—"}
                </td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function TaxSummaryTab() {
  const [fromDate, setFromDate] = useState(defaultFromDate());
  const [toDate, setToDate] = useState(toDateInputValue(new Date()));
  const [rows, setRows] = useState<TaxSummaryRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getTaxSummaryReport({ fromDate, toDate }));
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const totalTaxable = rows?.reduce((sum, r) => sum + r.taxableAmount, 0) ?? 0;
  const totalTax = rows?.reduce((sum, r) => sum + r.taxAmount, 0) ?? 0;

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      `tax-summary_${fromDate}_to_${toDate}.csv`,
      ["Tax Type", "Rate", "Transactions", "Taxable Amount", "Tax Amount", "Total"],
      rows.map((r) => [r.taxType, r.taxRate, r.transactionCount, r.taxableAmount.toFixed(2), r.taxAmount.toFixed(2), r.totalAmount.toFixed(2)]),
    );
  }

  return (
    <div className="space-y-4">
      <DateRangeFilter
        fromDate={fromDate}
        toDate={toDate}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        loading={loading}
        onGenerate={runReport}
      >
        <ExportCsvButton onClick={exportCsv} disabled={!rows || rows.length === 0} />
      </DateRangeFilter>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3">
        <SummaryItem label="Taxable Amount" value={currency.format(totalTaxable)} bold />
        <SummaryItem label="Total Tax" value={currency.format(totalTax)} bold />
        <SummaryItem label="Total (incl. tax)" value={currency.format(totalTaxable + totalTax)} bold />
      </div>

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Tax Type</th>
            <th className="px-5 py-3 font-medium">Rate</th>
            <th className="px-5 py-3 font-medium">Transactions</th>
            <th className="px-5 py-3 font-medium">Taxable Amount</th>
            <th className="px-5 py-3 font-medium">Tax Amount</th>
            <th className="px-5 py-3 font-medium">Total</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={6} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={6} message="No taxed transactions found in the selected range." />
          )}
          {!loading &&
            rows?.map((r) => (
              <tr key={r.taxId ?? "unspecified"} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.taxType}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.taxRate}%</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.transactionCount}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{currency.format(r.taxableAmount)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{currency.format(r.taxAmount)}</td>
                <td className="px-5 py-3.5 font-semibold text-slate-800 dark:text-slate-200">{currency.format(r.totalAmount)}</td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function PaymentTypeTab() {
  const [fromDate, setFromDate] = useState(defaultFromDate());
  const [toDate, setToDate] = useState(toDateInputValue(new Date()));
  const [rows, setRows] = useState<PaymentTypeCollectionRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getPaymentTypeCollectionReport({ fromDate, toDate }));
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const total = rows?.reduce((sum, r) => sum + r.totalAmount, 0) ?? 0;

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      `payment-type-collection_${fromDate}_to_${toDate}.csv`,
      ["Payment Type", "Transactions", "Total Amount"],
      rows.map((r) => [r.paymentTypeName, r.transactionCount, r.totalAmount.toFixed(2)]),
    );
  }

  return (
    <div className="space-y-4">
      <DateRangeFilter
        fromDate={fromDate}
        toDate={toDate}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        loading={loading}
        onGenerate={runReport}
      >
        <ExportCsvButton onClick={exportCsv} disabled={!rows || rows.length === 0} />
      </DateRangeFilter>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <SummaryItem label="Total Collected" value={currency.format(total)} bold />

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Payment Type</th>
            <th className="px-5 py-3 font-medium">Transactions</th>
            <th className="px-5 py-3 font-medium">Total Amount</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={3} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={3} message="No collections found in the selected range." />
          )}
          {!loading &&
            rows?.map((r) => (
              <tr key={r.paymentTypeId ?? "unspecified"} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.paymentTypeName}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.transactionCount}</td>
                <td className="px-5 py-3.5 font-semibold text-slate-800 dark:text-slate-200">{currency.format(r.totalAmount)}</td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function StaffCollectionTab() {
  const [fromDate, setFromDate] = useState(defaultFromDate());
  const [toDate, setToDate] = useState(toDateInputValue(new Date()));
  const [rows, setRows] = useState<StaffCollectionRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getStaffCollectionReport({ fromDate, toDate }));
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      `staff-collection_${fromDate}_to_${toDate}.csv`,
      ["Staff", "Receipts", "Receipt Total", "Refunds", "Refund Total", "Net"],
      rows.map((r) => [
        r.staffName,
        r.receiptCount,
        r.receiptTotal.toFixed(2),
        r.refundCount,
        r.refundTotal.toFixed(2),
        (r.receiptTotal - r.refundTotal).toFixed(2),
      ]),
    );
  }

  return (
    <div className="space-y-4">
      <DateRangeFilter
        fromDate={fromDate}
        toDate={toDate}
        onFromDateChange={setFromDate}
        onToDateChange={setToDate}
        loading={loading}
        onGenerate={runReport}
      >
        <ExportCsvButton onClick={exportCsv} disabled={!rows || rows.length === 0} />
      </DateRangeFilter>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Staff</th>
            <th className="px-5 py-3 font-medium">Receipts</th>
            <th className="px-5 py-3 font-medium">Receipt Total</th>
            <th className="px-5 py-3 font-medium">Refunds</th>
            <th className="px-5 py-3 font-medium">Refund Total</th>
            <th className="px-5 py-3 font-medium">Net</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={6} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={6} message="No staff activity found in the selected range." />
          )}
          {!loading &&
            rows?.map((r) => (
              <tr key={r.userId ?? "unassigned"} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.staffName}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.receiptCount}</td>
                <td className="px-5 py-3.5 text-emerald-600 dark:text-emerald-400">{currency.format(r.receiptTotal)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.refundCount}</td>
                <td className="px-5 py-3.5 text-red-600 dark:text-red-400">{currency.format(r.refundTotal)}</td>
                <td className="px-5 py-3.5 font-semibold text-slate-800 dark:text-slate-200">
                  {currency.format(r.receiptTotal - r.refundTotal)}
                </td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function RenewalStatusTab() {
  const [daysAhead, setDaysAhead] = useState(7);
  const [rows, setRows] = useState<RenewalStatusRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getRenewalStatusReport(daysAhead));
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      `renewal-status_due-${daysAhead}-days.csv`,
      ["Member No", "Name", "Workout", "Membership Type", "Next Renewal", "Days", "Status"],
      rows.map((r) => [
        r.memberNo,
        r.fullName,
        r.workOutName ?? "",
        r.membershipTypeName ?? "",
        formatDate(r.nextRenewalDate),
        r.daysRemaining,
        r.statusLabel,
      ]),
    );
  }

  return (
    <div className="space-y-4">
      <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-wrap items-end gap-4">
          <Field label="Due Within (days)" className="w-44">
            <input
              type="number"
              min={0}
              value={daysAhead}
              onChange={(e) => setDaysAhead(Number(e.target.value))}
              className={`${fieldInputClass} ${inputBorderClass(false)}`}
            />
          </Field>
          <GenerateButton loading={loading} onClick={runReport} />
          <ExportCsvButton onClick={exportCsv} disabled={!rows || rows.length === 0} />
        </div>
      </div>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Member No</th>
            <th className="px-5 py-3 font-medium">Name</th>
            <th className="px-5 py-3 font-medium">Workout</th>
            <th className="px-5 py-3 font-medium">Membership Type</th>
            <th className="px-5 py-3 font-medium">Next Renewal</th>
            <th className="px-5 py-3 font-medium">Days</th>
            <th className="px-5 py-3 font-medium">Status</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={7} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={7} message="No due or lapsed members found." />
          )}
          {!loading &&
            rows?.map((r) => (
              <tr key={r.memberId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{r.memberNo}</td>
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.fullName}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.workOutName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.membershipTypeName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(r.nextRenewalDate)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                  {r.daysRemaining < 0 ? `${Math.abs(r.daysRemaining)} overdue` : `${r.daysRemaining}`}
                </td>
                <td className="px-5 py-3.5">
                  <span
                    className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${
                      r.statusLabel === "Lapsed"
                        ? "bg-red-50 text-red-600 dark:bg-red-500/10 dark:text-red-300"
                        : "bg-amber-50 text-amber-600 dark:bg-amber-500/10 dark:text-amber-300"
                    }`}
                  >
                    {r.statusLabel}
                  </span>
                </td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}

function OutstandingBalancesTab() {
  const [rows, setRows] = useState<OutstandingBalanceRow[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function runReport() {
    setLoading(true);
    setError(null);
    try {
      setRows(await getOutstandingBalancesReport());
    } catch (err) {
      setRows([]);
      setError(err instanceof ApiError ? err.message : "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    runReport();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const totalOutstanding = rows?.reduce((sum, r) => sum + r.balanceDue, 0) ?? 0;

  function exportCsv() {
    if (!rows || rows.length === 0) return;
    downloadCsv(
      "outstanding-balances.csv",
      ["Member No", "Name", "Mobile", "Membership Type", "Invoice No", "Total", "Paid", "Balance Due", "Next Renewal"],
      rows.map((r) => [
        r.memberNo,
        r.fullName,
        r.mobileNo,
        r.membershipTypeName ?? "",
        r.invoiceNo,
        r.totalAmount.toFixed(2),
        r.amountPaid.toFixed(2),
        r.balanceDue.toFixed(2),
        formatDate(r.nextRenewalDate),
      ]),
    );
  }

  return (
    <div className="space-y-4">
      <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-wrap items-end gap-4">
          <GenerateButton loading={loading} onClick={runReport} />
          <ExportCsvButton onClick={exportCsv} disabled={!rows || rows.length === 0} />
        </div>
      </div>

      {error && <ErrorBanner message={error} onRetry={runReport} />}

      <SummaryItem label="Total Outstanding" value={currency.format(totalOutstanding)} bold />

      <ReportCard>
        <thead>
          <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
            <th className="px-5 py-3 font-medium">Member No</th>
            <th className="px-5 py-3 font-medium">Name</th>
            <th className="px-5 py-3 font-medium">Mobile</th>
            <th className="px-5 py-3 font-medium">Membership Type</th>
            <th className="px-5 py-3 font-medium">Invoice No</th>
            <th className="px-5 py-3 font-medium">Total</th>
            <th className="px-5 py-3 font-medium">Paid</th>
            <th className="px-5 py-3 font-medium">Balance Due</th>
            <th className="px-5 py-3 font-medium">Next Renewal</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
          {loading && <TableSkeleton colSpan={9} />}
          {!loading && rows !== null && rows.length === 0 && !error && (
            <EmptyRow colSpan={9} message="No outstanding balances — everyone is paid up." />
          )}
          {!loading &&
            rows?.map((r) => (
              <tr key={r.paymentId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{r.memberNo}</td>
                <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{r.fullName}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.mobileNo}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.membershipTypeName ?? "—"}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{r.invoiceNo}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{currency.format(r.totalAmount)}</td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{currency.format(r.amountPaid)}</td>
                <td className="px-5 py-3.5 font-semibold text-amber-600 dark:text-amber-400">
                  {currency.format(r.balanceDue)}
                </td>
                <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(r.nextRenewalDate)}</td>
              </tr>
            ))}
        </tbody>
      </ReportCard>
    </div>
  );
}
