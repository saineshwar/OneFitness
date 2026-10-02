"use client";

import { Suspense, useCallback, useEffect, useRef, useState } from "react";
import { useSearchParams } from "next/navigation";
import { MemberPicker } from "@/components/MemberPicker";
import { Pagination } from "@/components/Pagination";
import { Icon } from "@/components/icons";
import { usePagedList } from "@/hooks/usePagedList";
import { getMembers, type Member } from "@/lib/member";
import { getLatestPaymentByMember, type Payment } from "@/lib/payment";
import { generateReceipt, getReceiptById, getReceiptHistoryPaged, type Receipt, type ReceiptHistory } from "@/lib/receipt";
import { ApiError } from "@/lib/api";

const currency = new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR", maximumFractionDigits: 2 });

function formatDate(value: string | null) {
  if (!value) return "—";
  return new Date(value).toLocaleDateString("en-IN", { year: "numeric", month: "short", day: "numeric" });
}

export default function ReceiptsPage() {
  return (
    <Suspense fallback={null}>
      <ReceiptsPageInner />
    </Suspense>
  );
}

function ReceiptsPageInner() {
  const searchParams = useSearchParams();
  const [members, setMembers] = useState<Member[]>([]);

  const {
    items: history,
    loading: historyLoading,
    loadError: historyError,
    reload: reloadHistory,
    page: historyPage,
    setPage: setHistoryPage,
    totalPages: historyTotalPages,
    totalCount: historyTotalCount,
    pageSize: historyPageSize,
    searchInput: historySearchInput,
    setSearchInput: setHistorySearchInput,
  } = usePagedList(getReceiptHistoryPaged);

  useEffect(() => {
    getMembers().then(setMembers).catch(() => setMembers([]));
  }, []);

  const [selectedMember, setSelectedMember] = useState<Member | null>(null);
  const [selectedMemberPayment, setSelectedMemberPayment] = useState<Payment | null>(null);
  const [receipt, setReceipt] = useState<Receipt | null>(null);
  const [receiptLoading, setReceiptLoading] = useState(false);
  const [receiptError, setReceiptError] = useState<string | null>(null);
  const invoiceRef = useRef<HTMLDivElement>(null);

  const [memberInvoices, setMemberInvoices] = useState<ReceiptHistory[] | null>(null);
  const [memberInvoicesError, setMemberInvoicesError] = useState<string | null>(null);

  const loadMemberInvoices = useCallback(async (member: Member) => {
    setMemberInvoices(null);
    setMemberInvoicesError(null);
    try {
      const result = await getReceiptHistoryPaged({ page: 1, pageSize: 100, search: member.memberNo });
      setMemberInvoices(result.items);
    } catch (error) {
      setMemberInvoices([]);
      setMemberInvoicesError(error instanceof ApiError ? error.message : "Failed to load invoices.");
    }
  }, []);

  useEffect(() => {
    if (selectedMember) {
      loadMemberInvoices(selectedMember);
    } else {
      setMemberInvoices(null);
      setMemberInvoicesError(null);
    }
  }, [selectedMember, loadMemberInvoices]);

  function handleSelectMember(member: Member) {
    setSelectedMember(member);
    setSelectedMemberPayment(null);
    getLatestPaymentByMember(member.memberId)
      .then(setSelectedMemberPayment)
      .catch(() => setSelectedMemberPayment(null));
    setReceipt(null);
    setReceiptError(null);
  }

  function handleClearMember() {
    setSelectedMember(null);
    setSelectedMemberPayment(null);
    setReceipt(null);
    setReceiptError(null);
  }

  function scrollToInvoice() {
    requestAnimationFrame(() => invoiceRef.current?.scrollIntoView({ behavior: "smooth", block: "start" }));
  }

  async function handleGenerate() {
    if (!selectedMember) return;
    setReceiptLoading(true);
    setReceiptError(null);
    try {
      const result = await generateReceipt(selectedMember.memberId);
      setReceipt(result);
      await Promise.all([reloadHistory(), loadMemberInvoices(selectedMember)]);
      scrollToInvoice();
    } catch (error) {
      setReceiptError(error instanceof ApiError ? error.message : "Failed to generate receipt.");
    } finally {
      setReceiptLoading(false);
    }
  }

  async function handleView(receiptHistoryId: number) {
    setReceiptLoading(true);
    setReceiptError(null);
    try {
      const result = await getReceiptById(receiptHistoryId);
      setReceipt(result);
      scrollToInvoice();
    } catch (error) {
      setReceiptError(error instanceof ApiError ? error.message : "Failed to load that invoice.");
    } finally {
      setReceiptLoading(false);
    }
  }

  useEffect(() => {
    const historyId = searchParams.get("historyId");
    const id = historyId ? Number(historyId) : null;
    if (id && Number.isFinite(id)) {
      handleView(id);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchParams]);

  useEffect(() => {
    if (!receipt || selectedMember || members.length === 0) return;
    const match = members.find((m) => m.memberNo === receipt.memberNo);
    if (match) handleSelectMember(match);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [receipt, members]);

  return (
    <div className="mx-auto max-w-4xl space-y-6">
      <div className="print:hidden">
        <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Generate Receipt</h1>
        <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
          View, print, or reprint any invoice for a member — current or past.
        </p>
      </div>

      <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm print:hidden dark:border-slate-800 dark:bg-slate-900">
        <h2 className="mb-3 text-sm font-semibold text-slate-700 dark:text-slate-300">Select Member</h2>
        <MemberPicker members={members} selected={selectedMember} onSelect={handleSelectMember} onClear={handleClearMember} />

        {selectedMember && (
          <>
            <div className="mt-4 grid grid-cols-2 gap-3 rounded-lg border border-slate-200 bg-slate-50 px-4 py-3 text-sm sm:grid-cols-4 dark:border-slate-800 dark:bg-slate-800/50">
              <SummaryItem label="Amount" value={selectedMemberPayment ? currency.format(selectedMemberPayment.amount) : "—"} />
              <SummaryItem
                label="Tax"
                value={selectedMemberPayment ? currency.format(selectedMemberPayment.taxPercentageAmount) : "—"}
              />
              <SummaryItem
                label="Total"
                value={selectedMemberPayment ? currency.format(selectedMemberPayment.totalAmount) : "—"}
                bold
              />
              <SummaryItem label="Current Invoice No" value={selectedMemberPayment?.invoiceNo?.toString() ?? "—"} />
              {selectedMemberPayment && selectedMemberPayment.balanceDue > 0 && (
                <SummaryItem label="Balance Due" value={currency.format(selectedMemberPayment.balanceDue)} bold />
              )}
            </div>

            {receiptError && (
              <p className="mt-4 rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700 dark:bg-red-950 dark:text-red-300">
                {receiptError}
              </p>
            )}

            <div className="mt-4 flex justify-end">
              <button
                type="button"
                onClick={handleGenerate}
                disabled={receiptLoading || !selectedMemberPayment}
                title={!selectedMemberPayment ? "This member has no payment on file yet." : undefined}
                className="flex items-center gap-2 rounded-lg bg-indigo-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {receiptLoading && <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />}
                {receiptLoading ? "Working…" : "Generate Receipt for Current Plan"}
              </button>
            </div>

            <div className="mt-5 border-t border-slate-200 pt-4 dark:border-slate-800">
              <h3 className="mb-2 text-sm font-semibold text-slate-700 dark:text-slate-300">
                {[selectedMember.firstName, selectedMember.middleName, selectedMember.lastName].filter(Boolean).join(" ")}
                &apos;s Invoices
              </h3>
              {memberInvoicesError && (
                <p className="mb-2 rounded-lg bg-red-50 px-3 py-2 text-xs text-red-700 dark:bg-red-950 dark:text-red-300">
                  {memberInvoicesError}
                </p>
              )}
              <div className="overflow-x-auto rounded-lg border border-slate-200 dark:border-slate-800">
                <table className="w-full text-left text-sm">
                  <thead>
                    <tr className="border-b border-slate-100 bg-slate-50 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800 dark:bg-slate-800/50">
                      <th className="px-4 py-2.5 font-medium">Invoice No</th>
                      <th className="px-4 py-2.5 font-medium">Date</th>
                      <th className="px-4 py-2.5 font-medium text-right">Actions</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
                    {memberInvoices === null &&
                      [0, 1].map((i) => (
                        <tr key={i}>
                          <td className="px-4 py-3" colSpan={3}>
                            <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                          </td>
                        </tr>
                      ))}
                    {memberInvoices !== null && memberInvoices.length === 0 && (
                      <tr>
                        <td colSpan={3} className="px-4 py-6 text-center text-sm text-slate-400">
                          No invoices for this member yet.
                        </td>
                      </tr>
                    )}
                    {memberInvoices?.map((row) => (
                      <tr key={row.receiptHistoryId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                        <td className="px-4 py-3 font-medium text-slate-800 dark:text-slate-200">{row.invoiceNo ?? "—"}</td>
                        <td className="px-4 py-3 text-slate-500 dark:text-slate-400">{formatDate(row.createdOn)}</td>
                        <td className="px-4 py-3 text-right">
                          <button
                            type="button"
                            onClick={() => handleView(row.receiptHistoryId)}
                            disabled={receiptLoading}
                            className="inline-flex items-center gap-1.5 rounded-lg border border-slate-300 px-2.5 py-1 text-xs font-medium text-slate-600 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800"
                          >
                            <Icon name="eye" className="h-3.5 w-3.5" />
                            View / Print
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </>
        )}
      </div>

      {receipt && (
        <div ref={invoiceRef}>
          <div className="mb-3 flex items-center justify-end print:hidden">
            <button
              type="button"
              onClick={() => window.print()}
              className="flex items-center gap-2 rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 transition hover:bg-slate-50 dark:border-slate-700 dark:text-slate-200 dark:hover:bg-slate-800"
            >
              <Icon name="print" className="h-4 w-4" />
              Print
            </button>
          </div>

          <div id="print-area" className="rounded-xl border border-slate-200/70 bg-white p-8 shadow-sm dark:border-slate-800 dark:bg-slate-900">
            <div className="flex items-start justify-between border-b border-slate-200 pb-4 dark:border-slate-800">
              <div>
                <h2 className="text-lg font-semibold tracking-tight text-slate-900 dark:text-white">
                  {receipt.companyName ?? "OneFitness"}
                </h2>
                {receipt.companyAddress && (
                  <p className="mt-1 max-w-xs text-sm text-slate-500 dark:text-slate-400">{receipt.companyAddress}</p>
                )}
                <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
                  {[receipt.companySupportEmailId, receipt.companyTelephoneNo].filter(Boolean).join(" · ")}
                </p>
              </div>
              <div className="text-right">
                <p className="text-sm font-semibold text-slate-900 dark:text-white">Invoice #{receipt.invoiceNo ?? "—"}</p>
                <p className="text-sm text-slate-500 dark:text-slate-400">{formatDate(receipt.invoiceDate)}</p>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4 border-b border-slate-200 py-4 text-sm sm:grid-cols-4 dark:border-slate-800">
              <SummaryItem label="Member No" value={receipt.memberNo} />
              <SummaryItem
                label="Name"
                value={[receipt.firstName, receipt.middleName, receipt.lastName].filter(Boolean).join(" ")}
              />
              <SummaryItem label="Payment From" value={formatDate(receipt.paymentFromDate)} />
              <SummaryItem label="Next Renewal" value={formatDate(receipt.nextRenewalDate)} />
            </div>

            {receipt.amount == null && (
              <p className="mt-3 rounded-lg bg-amber-50 px-3 py-2 text-xs text-amber-700 dark:bg-amber-950 dark:text-amber-300">
                Plan and amount details were not recorded for this invoice — it was generated before per-invoice history was
                tracked.
              </p>
            )}

            <table className="w-full text-left text-sm">
              <thead>
                <tr className="border-b border-slate-200 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
                  <th className="py-2 font-medium">Description</th>
                  <th className="py-2 font-medium">Workout</th>
                  <th className="py-2 font-medium">Installment</th>
                  <th className="py-2 font-medium">Payment Type</th>
                  <th className="py-2 text-right font-medium">Amount</th>
                </tr>
              </thead>
              <tbody>
                <tr className="border-b border-slate-100 dark:border-slate-800">
                  <td className="py-3 text-slate-800 dark:text-slate-200">{receipt.membershipTypeName ?? "—"}</td>
                  <td className="py-3 text-slate-500 dark:text-slate-400">{receipt.workOutName ?? "—"}</td>
                  <td className="py-3 text-slate-500 dark:text-slate-400">{receipt.installmentName ?? "—"}</td>
                  <td className="py-3 text-slate-500 dark:text-slate-400">{receipt.paymentTypeName ?? "—"}</td>
                  <td className="py-3 text-right text-slate-800 dark:text-slate-200">
                    {receipt.amount != null ? currency.format(receipt.amount) : "—"}
                  </td>
                </tr>
              </tbody>
            </table>

            <div className="mt-4 flex justify-end">
              <div className="w-full max-w-xs space-y-1.5 text-sm">
                <div className="flex justify-between text-slate-500 dark:text-slate-400">
                  <span>Subtotal</span>
                  <span>{receipt.amount != null ? currency.format(receipt.amount) : "—"}</span>
                </div>
                <div className="flex justify-between text-slate-500 dark:text-slate-400">
                  <span>
                    Tax{receipt.taxType ? ` (${receipt.taxType}${receipt.taxRate != null ? ` ${receipt.taxRate}%` : ""})` : ""}
                  </span>
                  <span>{receipt.taxPercentageAmount != null ? currency.format(receipt.taxPercentageAmount) : "—"}</span>
                </div>
                <div className="flex justify-between border-t border-slate-200 pt-1.5 font-semibold text-slate-900 dark:border-slate-800 dark:text-white">
                  <span>Total</span>
                  <span>{receipt.totalAmount != null ? currency.format(receipt.totalAmount) : "—"}</span>
                </div>
                {receipt.amountPaid != null && (
                  <div className="flex justify-between text-slate-500 dark:text-slate-400">
                    <span>Amount Paid</span>
                    <span>{currency.format(receipt.amountPaid)}</span>
                  </div>
                )}
                {receipt.balanceDue != null && receipt.balanceDue > 0 && (
                  <div className="flex justify-between font-semibold text-amber-600 dark:text-amber-400">
                    <span>Balance Due</span>
                    <span>{currency.format(receipt.balanceDue)}</span>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      )}

      <div className="rounded-xl border border-slate-200/70 bg-white shadow-sm print:hidden dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-col gap-3 border-b border-slate-100 px-5 py-4 sm:flex-row sm:items-center sm:justify-between dark:border-slate-800">
          <h2 className="text-base font-semibold tracking-tight text-slate-900 dark:text-white">All Receipts</h2>
          <div className="relative">
            <Icon name="search" className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              value={historySearchInput}
              onChange={(e) => setHistorySearchInput(e.target.value)}
              placeholder="Search by member no…"
              className="w-full max-w-xs rounded-lg border border-slate-300 bg-white py-2 pr-3 pl-9 text-sm text-slate-900 outline-none transition focus:border-indigo-500 focus:ring-4 focus:ring-indigo-500/10 dark:border-slate-700 dark:bg-slate-900 dark:text-white"
            />
          </div>
        </div>

        {historyError && (
          <div className="flex items-center justify-between gap-4 px-5 py-3 text-sm text-red-700 dark:text-red-300">
            <span>{historyError}</span>
            <button
              type="button"
              onClick={reloadHistory}
              className="shrink-0 rounded-md bg-red-50 px-2.5 py-1 text-xs font-medium text-red-700 transition hover:bg-red-100 dark:bg-red-950 dark:hover:bg-red-900"
            >
              Retry
            </button>
          </div>
        )}

        <div className={`overflow-x-auto transition-opacity ${historyLoading && history !== null ? "opacity-60" : ""}`}>
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
                <th className="px-5 py-3 font-medium">Invoice No</th>
                <th className="px-5 py-3 font-medium">Member No</th>
                <th className="px-5 py-3 font-medium">Date</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {history === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={4}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {history !== null && history.length === 0 && !historyError && (
                <tr>
                  <td colSpan={4} className="px-5 py-10 text-center text-sm text-slate-400">
                    {historySearchInput ? "No receipts match your search." : "No receipts generated yet."}
                  </td>
                </tr>
              )}

              {history?.map((row) => (
                <tr key={row.receiptHistoryId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{row.invoiceNo ?? "—"}</td>
                  <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{row.memberNo}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(row.createdOn)}</td>
                  <td className="px-5 py-3.5 text-right">
                    <button
                      type="button"
                      onClick={() => handleView(row.receiptHistoryId)}
                      disabled={receiptLoading}
                      className="inline-flex items-center gap-1.5 rounded-lg border border-slate-300 px-2.5 py-1 text-xs font-medium text-slate-600 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800"
                    >
                      <Icon name="eye" className="h-3.5 w-3.5" />
                      View / Print
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <Pagination
          page={historyPage}
          totalPages={historyTotalPages}
          totalCount={historyTotalCount}
          pageSize={historyPageSize}
          onPageChange={setHistoryPage}
        />
      </div>
    </div>
  );
}

function SummaryItem({ label, value, bold }: { label: string; value: string; bold?: boolean }) {
  return (
    <div>
      <p className="text-[11px] text-slate-400">{label}</p>
      <p className={`text-slate-800 dark:text-slate-200 ${bold ? "font-semibold" : ""}`}>{value}</p>
    </div>
  );
}
