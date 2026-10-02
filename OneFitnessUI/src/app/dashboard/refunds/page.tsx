"use client";

import { useEffect, useState, type FormEvent } from "react";
import { Modal } from "@/components/Modal";
import { ConfirmDeleteModal } from "@/components/ConfirmDeleteModal";
import { StatusBadge } from "@/components/StatusBadge";
import { Field } from "@/components/Field";
import { MemberPicker } from "@/components/MemberPicker";
import { Pagination } from "@/components/Pagination";
import { Icon } from "@/components/icons";
import { usePagedList } from "@/hooks/usePagedList";
import { getMembers, type Member } from "@/lib/member";
import { getLatestPaymentByMember, type Payment } from "@/lib/payment";
import { createRefund, deleteRefund, getRefundsPaged, type Refund } from "@/lib/refund";
import { ApiError } from "@/lib/api";
import { collectErrors, type FieldErrors } from "@/lib/validation";

const currency = new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR", maximumFractionDigits: 0 });

function formatDate(value: string) {
  return new Date(value).toLocaleDateString("en-IN", { year: "numeric", month: "short", day: "numeric" });
}

export default function RefundPage() {
  const {
    items: refunds,
    loading: refundsLoading,
    loadError,
    reload: loadRefunds,
    page,
    setPage,
    totalPages,
    totalCount,
    pageSize,
    searchInput,
    setSearchInput,
  } = usePagedList(getRefundsPaged);

  const [members, setMembers] = useState<Member[]>([]);

  useEffect(() => {
    getMembers().then(setMembers).catch(() => setMembers([]));
  }, []);

  const [modalOpen, setModalOpen] = useState(false);
  const [selectedMember, setSelectedMember] = useState<Member | null>(null);
  const [selectedMemberPayment, setSelectedMemberPayment] = useState<Payment | null>(null);
  const [amount, setAmount] = useState("");
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const [deleteTarget, setDeleteTarget] = useState<Refund | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  function openCreate() {
    setSelectedMember(null);
    setSelectedMemberPayment(null);
    setAmount("");
    setFieldErrors({});
    setFormError(null);
    setModalOpen(true);
  }

  function handleSelectMember(member: Member) {
    setSelectedMember(member);
    setSelectedMemberPayment(null);
    getLatestPaymentByMember(member.memberId)
      .then(setSelectedMemberPayment)
      .catch(() => setSelectedMemberPayment(null));
  }

  function handleClearMember() {
    setSelectedMember(null);
    setSelectedMemberPayment(null);
  }

  function closeModal() {
    if (saving) return;
    setModalOpen(false);
  }

  function validate(): FieldErrors {
    return collectErrors({
      member: selectedMember ? undefined : "Please select a member first.",
      amount: !amount.trim() ? "Refund amount is required." : Number(amount) <= 0 ? "Amount must be greater than 0." : undefined,
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0 || !selectedMember) return;

    setSaving(true);
    setFormError(null);
    try {
      await createRefund({ memberId: selectedMember.memberId, amount: Number(amount) });
      setModalOpen(false);
      await loadRefunds();
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : "Failed to create refund.");
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    setDeleteError(null);
    try {
      await deleteRefund(deleteTarget.refundId);
      setDeleteTarget(null);
      await loadRefunds();
    } catch (error) {
      setDeleteError(error instanceof ApiError ? error.message : "Failed to delete refund.");
    } finally {
      setDeleting(false);
    }
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Refunds</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
            Issue and track refunds against a member&apos;s membership payment.
          </p>
        </div>
        <button
          type="button"
          onClick={openCreate}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500"
        >
          <span className="text-base leading-none">+</span>
          Add Refund
        </button>
      </div>

      {loadError && (
        <div className="flex items-center justify-between gap-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300">
          <span>{loadError}</span>
          <button
            type="button"
            onClick={loadRefunds}
            className="shrink-0 rounded-md bg-white px-2.5 py-1 text-xs font-medium text-red-700 shadow-sm transition hover:bg-red-100 dark:bg-slate-900 dark:hover:bg-slate-800"
          >
            Retry
          </button>
        </div>
      )}

      <div className="relative">
        <Icon name="search" className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
        <input
          type="text"
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
          placeholder="Search by member name or number…"
          className="w-full max-w-sm rounded-lg border border-slate-300 bg-white py-2.5 pr-3 pl-9 text-sm text-slate-900 outline-none transition focus:border-indigo-500 focus:ring-4 focus:ring-indigo-500/10 dark:border-slate-700 dark:bg-slate-900 dark:text-white"
        />
      </div>

      <div className="rounded-xl border border-slate-200/70 bg-white shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className={`overflow-x-auto transition-opacity ${refundsLoading && refunds !== null ? "opacity-60" : ""}`}>
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
                <th className="px-5 py-3 font-medium">Member No</th>
                <th className="px-5 py-3 font-medium">Member</th>
                <th className="px-5 py-3 font-medium">Amount</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium">Created</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {refunds === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={6}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {refunds !== null && refunds.length === 0 && !loadError && (
                <tr>
                  <td colSpan={6} className="px-5 py-10 text-center text-sm text-slate-400">
                    {searchInput ? "No refunds match your search." : "No refunds yet."}
                  </td>
                </tr>
              )}

              {refunds?.map((refund) => (
                <tr key={refund.refundId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{refund.memberNo}</td>
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{refund.memberFullName}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{currency.format(refund.amount)}</td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={refund.status} />
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(refund.createdOn)}</td>
                  <td className="px-5 py-3.5">
                    <div className="flex justify-end">
                      <button
                        type="button"
                        onClick={() => {
                          setDeleteError(null);
                          setDeleteTarget(refund);
                        }}
                        aria-label={`Delete refund for ${refund.memberFullName}`}
                        className="rounded-lg p-2 text-slate-400 transition hover:bg-red-50 hover:text-red-600 dark:hover:bg-red-950 dark:hover:text-red-300"
                      >
                        <Icon name="trash" className="h-4 w-4" />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <Pagination page={page} totalPages={totalPages} totalCount={totalCount} pageSize={pageSize} onPageChange={setPage} />
      </div>

      <Modal open={modalOpen} onClose={closeModal} title="Add Refund">
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <Field label="Member" error={fieldErrors.member}>
            <MemberPicker
              members={members}
              selected={selectedMember}
              onSelect={handleSelectMember}
              onClear={handleClearMember}
            />
          </Field>

          {selectedMember && (
            <div className="grid grid-cols-2 gap-3 rounded-lg border border-slate-200 bg-slate-50 px-4 py-3 text-sm sm:grid-cols-3 dark:border-slate-800 dark:bg-slate-800/50">
              <SummaryItem
                label="Amount Paid (refundable)"
                value={selectedMemberPayment ? currency.format(selectedMemberPayment.amountPaid) : "—"}
              />
              <SummaryItem
                label="Next Renewal"
                value={selectedMemberPayment ? formatDate(selectedMemberPayment.nextRenewalDate) : "—"}
              />
              <SummaryItem label="Status" value={selectedMember.status ? "Active" : "Inactive"} />
            </div>
          )}

          <Field label="Refund Amount" error={fieldErrors.amount}>
            <input
              type="number"
              min={0}
              step="0.01"
              value={amount}
              onChange={(event) => setAmount(event.target.value)}
              className="w-full rounded-lg border border-slate-300 bg-white px-3 py-2.5 text-sm text-slate-900 outline-none transition focus:border-indigo-500 focus:ring-4 focus:ring-indigo-500/10 dark:border-slate-700 dark:bg-slate-900 dark:text-white"
              placeholder="e.g. 2000"
            />
          </Field>

          {formError && (
            <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700 dark:bg-red-950 dark:text-red-300">
              {formError}
            </p>
          )}

          <div className="flex justify-end gap-2 pt-2">
            <button
              type="button"
              onClick={closeModal}
              className="rounded-lg px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-800"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={saving}
              className="flex items-center gap-2 rounded-lg bg-indigo-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-indigo-500 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {saving && <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />}
              {saving ? "Saving…" : "Save"}
            </button>
          </div>
        </form>
      </Modal>

      <ConfirmDeleteModal
        open={deleteTarget !== null}
        itemLabel={deleteTarget ? `refund for ${deleteTarget.memberFullName}` : ""}
        deleting={deleting}
        error={deleteError}
        onCancel={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title="Delete Refund"
      />
    </div>
  );
}

function SummaryItem({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <p className="text-[11px] text-slate-400">{label}</p>
      <p className="text-slate-800 dark:text-slate-200">{value}</p>
    </div>
  );
}

