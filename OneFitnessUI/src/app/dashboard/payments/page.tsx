"use client";

import { Suspense, useEffect, useState, type FormEvent } from "react";
import { useSearchParams } from "next/navigation";
import { Field, fieldInputClass, inputBorderClass } from "@/components/Field";
import { MemberPicker, memberFullName } from "@/components/MemberPicker";
import { Pagination } from "@/components/Pagination";
import { Icon } from "@/components/icons";
import { usePagedList } from "@/hooks/usePagedList";
import { getMembers, type Member } from "@/lib/member";
import {
  calculatePaymentAmount,
  collectPaymentBalance,
  createPayment,
  getLatestPaymentByMember,
  getPaymentsPaged,
  type Payment,
  type PaymentAmountCalculation,
} from "@/lib/payment";
import { getWorkOuts, type WorkOut } from "@/lib/workOut";
import { getInstallments, type Installment } from "@/lib/installment";
import { getMembershipTypes, type MembershipType } from "@/lib/membershipType";
import { getPaymentTypes, type PaymentType } from "@/lib/paymentType";
import { getTaxMasters, type TaxMaster } from "@/lib/taxMaster";
import { ApiError } from "@/lib/api";
import { collectErrors, requiredId, type FieldErrors } from "@/lib/validation";

const currency = new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR", maximumFractionDigits: 0 });

function formatDate(value: string | null) {
  if (!value) return "—";
  return new Date(value).toLocaleDateString("en-IN", { year: "numeric", month: "short", day: "numeric" });
}

const emptyForm = {
  workOutId: "",
  installmentId: "",
  membershipTypeId: "",
  paymentTypeId: "",
  taxId: "",
  amountPaid: "",
};

export default function PaymentsPage() {
  return (
    <Suspense fallback={null}>
      <PaymentsPageInner />
    </Suspense>
  );
}

function PaymentsPageInner() {
  const searchParams = useSearchParams();

  const [members, setMembers] = useState<Member[]>([]);
  const [workOuts, setWorkOuts] = useState<WorkOut[]>([]);
  const [installments, setInstallments] = useState<Installment[]>([]);
  const [membershipTypes, setMembershipTypes] = useState<MembershipType[]>([]);
  const [paymentTypes, setPaymentTypes] = useState<PaymentType[]>([]);
  const [taxMasters, setTaxMasters] = useState<TaxMaster[]>([]);

  useEffect(() => {
    getMembers().then(setMembers).catch(() => setMembers([]));
    getWorkOuts().then(setWorkOuts).catch(() => setWorkOuts([]));
    getInstallments().then(setInstallments).catch(() => setInstallments([]));
    getMembershipTypes().then(setMembershipTypes).catch(() => setMembershipTypes([]));
    getPaymentTypes().then(setPaymentTypes).catch(() => setPaymentTypes([]));
    getTaxMasters().then(setTaxMasters).catch(() => setTaxMasters([]));
  }, []);

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
  } = usePagedList(getPaymentsPaged);

  const [selectedMember, setSelectedMember] = useState<Member | null>(null);
  const [currentPayment, setCurrentPayment] = useState<Payment | null>(null);
  const [currentPaymentLoading, setCurrentPaymentLoading] = useState(false);
  const [form, setForm] = useState(emptyForm);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [recorded, setRecorded] = useState<Payment | null>(null);

  const [calc, setCalc] = useState<PaymentAmountCalculation | null>(null);
  const [calculating, setCalculating] = useState(false);

  const [collectAmount, setCollectAmount] = useState("");
  const [collectError, setCollectError] = useState<string | null>(null);
  const [collecting, setCollecting] = useState(false);

  function updateField<K extends keyof typeof form>(key: K, value: (typeof form)[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function loadCurrentPayment(memberId: number) {
    setCurrentPaymentLoading(true);
    getLatestPaymentByMember(memberId)
      .then(setCurrentPayment)
      .catch(() => setCurrentPayment(null))
      .finally(() => setCurrentPaymentLoading(false));
  }

  function handleSelectMember(member: Member) {
    setSelectedMember(member);
    setRecorded(null);
    setFormError(null);
    setFieldErrors({});
    setForm(emptyForm);
    setCollectAmount("");
    setCollectError(null);
    loadCurrentPayment(member.memberId);
  }

  function handleClearMember() {
    setSelectedMember(null);
    setCurrentPayment(null);
    setRecorded(null);
    setForm(emptyForm);
    setFieldErrors({});
    setFormError(null);
    setCollectAmount("");
    setCollectError(null);
  }

  async function handleCollectBalance(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!currentPayment) return;

    const amount = Number(collectAmount);
    if (!collectAmount.trim() || amount <= 0) {
      setCollectError("Enter an amount greater than 0.");
      return;
    }
    if (amount > currentPayment.balanceDue) {
      setCollectError("Amount cannot exceed the remaining balance.");
      return;
    }

    setCollecting(true);
    setCollectError(null);
    try {
      const updated = await collectPaymentBalance(currentPayment.paymentId, amount);
      setCurrentPayment(updated);
      setCollectAmount("");
      await reloadHistory();
    } catch (error) {
      setCollectError(error instanceof ApiError ? error.message : "Failed to collect payment.");
    } finally {
      setCollecting(false);
    }
  }

  useEffect(() => {
    const memberIdParam = searchParams.get("memberId");
    const id = memberIdParam ? Number(memberIdParam) : null;
    if (id && Number.isFinite(id) && members.length > 0 && !selectedMember) {
      const match = members.find((m) => m.memberId === id);
      if (match) handleSelectMember(match);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchParams, members]);

  useEffect(() => {
    if (!form.membershipTypeId || !form.taxId) {
      setCalc(null);
      return;
    }
    let cancelled = false;
    setCalculating(true);
    calculatePaymentAmount(Number(form.membershipTypeId), Number(form.taxId))
      .then((result) => {
        if (!cancelled) {
          setCalc(result);
          updateField("amountPaid", String(result.totalAmount));
        }
      })
      .catch(() => {
        if (!cancelled) setCalc(null);
      })
      .finally(() => {
        if (!cancelled) setCalculating(false);
      });
    return () => {
      cancelled = true;
    };
  }, [form.membershipTypeId, form.taxId]);

  const filteredMembershipTypes = membershipTypes.filter(
    (m) => String(m.workOutId) === form.workOutId && String(m.installmentId) === form.installmentId,
  );

  function handleWorkOutChange(value: string) {
    updateField("workOutId", value);
    updateField("membershipTypeId", "");
  }

  function handleInstallmentChange(value: string) {
    updateField("installmentId", value);
    updateField("membershipTypeId", "");
  }

  function validate(): FieldErrors {
    return collectErrors({
      member: selectedMember ? undefined : "Please select a member first.",
      workOutId: requiredId(Number(form.workOutId), "Please select a workout."),
      installmentId: requiredId(Number(form.installmentId), "Please select an installment plan."),
      membershipTypeId: requiredId(Number(form.membershipTypeId), "Please select a membership type."),
      paymentTypeId: requiredId(Number(form.paymentTypeId), "Please select a payment type."),
      taxId: requiredId(Number(form.taxId), "Please select a tax rate."),
      amountPaid: !form.amountPaid.trim()
        ? "Please enter the amount being collected now."
        : Number(form.amountPaid) <= 0
          ? "Amount must be greater than 0."
          : calc && Number(form.amountPaid) > calc.totalAmount
            ? "Amount cannot exceed the total due."
            : undefined,
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
      const payment = await createPayment({
        memberId: selectedMember.memberId,
        workOutId: Number(form.workOutId),
        installmentId: Number(form.installmentId),
        membershipTypeId: Number(form.membershipTypeId),
        paymentTypeId: Number(form.paymentTypeId),
        taxId: Number(form.taxId),
        amountPaid: Number(form.amountPaid),
      });
      setRecorded(payment);
      setCurrentPayment(payment);
      setForm(emptyForm);
      setCalc(null);
      await reloadHistory();
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : "Failed to record payment.");
    } finally {
      setSaving(false);
    }
  }

  function lookupName<T extends { status: boolean }>(
    list: T[],
    id: number | null | undefined,
    idKey: keyof T,
    nameKey: keyof T,
  ) {
    const found = list.find((item) => item[idKey] === id);
    return found ? String(found[nameKey]) : "—";
  }

  return (
    <div className="mx-auto max-w-4xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Payments</h1>
        <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
          Record a member&apos;s first payment or a renewal. Every payment is tracked independently of the member
          record.
        </p>
      </div>

      <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <h2 className="mb-3 text-sm font-semibold text-slate-700 dark:text-slate-300">Select Member</h2>
        <MemberPicker members={members} selected={selectedMember} onSelect={handleSelectMember} onClear={handleClearMember} />
      </div>

      {selectedMember && (
        <>
          <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
            <h2 className="mb-3 text-sm font-semibold text-slate-700 dark:text-slate-300">Current Membership</h2>
            {currentPaymentLoading ? (
              <p className="text-sm text-slate-400">Loading…</p>
            ) : currentPayment ? (
              <div className="grid grid-cols-2 gap-4 text-sm sm:grid-cols-4">
                <SummaryItem
                  label="Membership Type"
                  value={lookupName(membershipTypes, currentPayment.membershipTypeId, "membershipTypeId", "membershipTypeName")}
                />
                <SummaryItem
                  label="Workout"
                  value={lookupName(workOuts, currentPayment.workOutId, "workOutId", "workOutName")}
                />
                <SummaryItem
                  label="Installment"
                  value={lookupName(installments, currentPayment.installmentId, "installmentId", "installmentName")}
                />
                <SummaryItem
                  label="Payment Type"
                  value={lookupName(paymentTypes, currentPayment.paymentTypeId, "paymentTypeId", "paymentTypeName")}
                />
                <SummaryItem label="Next Renewal" value={formatDate(currentPayment.nextRenewalDate)} />
                <SummaryItem label="Total Amount" value={currency.format(currentPayment.totalAmount)} />
                <SummaryItem label="Amount Paid" value={currency.format(currentPayment.amountPaid)} />
                <SummaryItem
                  label="Balance Due"
                  value={currentPayment.balanceDue > 0 ? currency.format(currentPayment.balanceDue) : "Fully paid"}
                  bold={currentPayment.balanceDue > 0}
                />
                <SummaryItem label="Invoice No" value={currentPayment.invoiceNo.toString()} />
                <SummaryItem label="Status" value={selectedMember.status ? "Active" : "Inactive"} />
              </div>
            ) : (
              <p className="text-sm text-slate-400">
                No payment on file yet — this will be {memberFullName(selectedMember)}&apos;s first payment.
              </p>
            )}

            {currentPayment && currentPayment.balanceDue > 0 && (
              <form onSubmit={handleCollectBalance} className="mt-4 flex flex-wrap items-end gap-3 border-t border-slate-200 pt-4 dark:border-slate-800">
                <div className="min-w-[160px]">
                  <label className="mb-1.5 block text-sm font-medium text-slate-700 dark:text-slate-300">
                    Collect balance ({currency.format(currentPayment.balanceDue)} due)
                  </label>
                  <input
                    type="number"
                    min={0}
                    max={currentPayment.balanceDue}
                    step="0.01"
                    value={collectAmount}
                    onChange={(e) => setCollectAmount(e.target.value)}
                    className={`${fieldInputClass} ${inputBorderClass(!!collectError)}`}
                    placeholder={String(currentPayment.balanceDue)}
                  />
                </div>
                <button
                  type="submit"
                  disabled={collecting}
                  className="flex items-center gap-2 rounded-lg bg-indigo-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-indigo-500 disabled:cursor-not-allowed disabled:opacity-60"
                >
                  {collecting && <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />}
                  {collecting ? "Collecting…" : "Collect"}
                </button>
                {collectError && <p className="w-full text-xs text-red-600 dark:text-red-400">{collectError}</p>}
              </form>
            )}
          </div>

          {recorded && (
            <div className="flex items-start gap-3 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700 dark:border-emerald-900 dark:bg-emerald-950 dark:text-emerald-300">
              <Icon name="check" className="mt-0.5 h-4 w-4 shrink-0" />
              <p>
                Payment recorded. New renewal date <strong>{formatDate(recorded.nextRenewalDate)}</strong>, invoice{" "}
                <strong>{recorded.invoiceNo}</strong>, paid <strong>{currency.format(recorded.amountPaid)}</strong> of{" "}
                <strong>{currency.format(recorded.totalAmount)}</strong>
                {recorded.balanceDue > 0 && (
                  <>
                    {" "}
                    — balance <strong>{currency.format(recorded.balanceDue)}</strong> still due.
                  </>
                )}
                {recorded.balanceDue <= 0 && "."}
              </p>
            </div>
          )}

          <div className="rounded-xl border border-slate-200/70 bg-white p-5 shadow-sm dark:border-slate-800 dark:bg-slate-900">
            <h2 className="mb-4 text-sm font-semibold text-slate-700 dark:text-slate-300">Record Payment</h2>
            <form onSubmit={handleSubmit} noValidate className="space-y-4">
              {fieldErrors.member && (
                <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700 dark:bg-red-950 dark:text-red-300">
                  {fieldErrors.member}
                </p>
              )}

              <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
                <Field label="Workout" error={fieldErrors.workOutId}>
                  <select
                    value={form.workOutId}
                    onChange={(e) => handleWorkOutChange(e.target.value)}
                    className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.workOutId)}`}
                  >
                    <option value="" disabled>
                      Select
                    </option>
                    {workOuts.map((w) => (
                      <option key={w.workOutId} value={w.workOutId}>
                        {w.workOutName}
                      </option>
                    ))}
                  </select>
                </Field>

                <Field label="Installment Plan" error={fieldErrors.installmentId}>
                  <select
                    value={form.installmentId}
                    onChange={(e) => handleInstallmentChange(e.target.value)}
                    className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.installmentId)}`}
                  >
                    <option value="" disabled>
                      Select
                    </option>
                    {installments.map((i) => (
                      <option key={i.installmentId} value={i.installmentId}>
                        {i.installmentName}
                      </option>
                    ))}
                  </select>
                </Field>

                <Field label="Membership Type" error={fieldErrors.membershipTypeId}>
                  <select
                    value={form.membershipTypeId}
                    onChange={(e) => updateField("membershipTypeId", e.target.value)}
                    disabled={!form.workOutId || !form.installmentId}
                    className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.membershipTypeId)} disabled:cursor-not-allowed disabled:bg-slate-50 dark:disabled:bg-slate-800`}
                  >
                    <option value="" disabled>
                      {form.workOutId && form.installmentId ? "Select" : "Select workout & installment first"}
                    </option>
                    {filteredMembershipTypes.map((m) => (
                      <option key={m.membershipTypeId} value={m.membershipTypeId}>
                        {m.membershipTypeName}
                      </option>
                    ))}
                  </select>
                </Field>

                <Field label="Payment Type" error={fieldErrors.paymentTypeId}>
                  <select
                    value={form.paymentTypeId}
                    onChange={(e) => updateField("paymentTypeId", e.target.value)}
                    className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.paymentTypeId)}`}
                  >
                    <option value="" disabled>
                      Select
                    </option>
                    {paymentTypes.map((p) => (
                      <option key={p.paymentTypeId} value={p.paymentTypeId}>
                        {p.paymentTypeName}
                      </option>
                    ))}
                  </select>
                </Field>

                <Field label="Tax" error={fieldErrors.taxId}>
                  <select
                    value={form.taxId}
                    onChange={(e) => updateField("taxId", e.target.value)}
                    className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.taxId)}`}
                  >
                    <option value="" disabled>
                      Select
                    </option>
                    {taxMasters.map((t) => (
                      <option key={t.taxId} value={t.taxId}>
                        {t.taxType} ({t.taxRate}%)
                      </option>
                    ))}
                  </select>
                </Field>
              </div>

              <div className="rounded-lg border border-slate-200 bg-slate-50 px-4 py-3 dark:border-slate-800 dark:bg-slate-800/50">
                <p className="mb-2 text-xs font-medium tracking-wide text-slate-500 uppercase dark:text-slate-400">
                  {calculating ? "Calculating…" : "Amount Summary"}
                </p>
                <div className="grid grid-cols-2 gap-2 text-sm sm:grid-cols-4">
                  <SummaryItem label="Amount" value={calc ? currency.format(calc.amount) : "—"} />
                  <SummaryItem label="Tax %" value={calc ? `${calc.taxPercentage}%` : "—"} />
                  <SummaryItem label="Tax Amount" value={calc ? currency.format(calc.taxPercentageAmount) : "—"} />
                  <SummaryItem label="Total Due" value={calc ? currency.format(calc.totalAmount) : "—"} bold />
                </div>
              </div>

              <Field label="Amount Collecting Now" error={fieldErrors.amountPaid}>
                <input
                  type="number"
                  min={0}
                  max={calc?.totalAmount}
                  step="0.01"
                  value={form.amountPaid}
                  onChange={(e) => updateField("amountPaid", e.target.value)}
                  disabled={!calc}
                  className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.amountPaid)} disabled:cursor-not-allowed disabled:bg-slate-50 dark:disabled:bg-slate-800`}
                />
              </Field>
              {calc && Number(form.amountPaid) > 0 && Number(form.amountPaid) < calc.totalAmount && (
                <p className="text-xs text-amber-600 dark:text-amber-400">
                  Partial payment — balance of {currency.format(calc.totalAmount - Number(form.amountPaid))} will remain
                  due.
                </p>
              )}

              {formError && (
                <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700 dark:bg-red-950 dark:text-red-300">
                  {formError}
                </p>
              )}

              <div className="flex justify-end">
                <button
                  type="submit"
                  disabled={saving}
                  className="flex items-center gap-2 rounded-lg bg-indigo-600 px-5 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500 disabled:cursor-not-allowed disabled:opacity-60"
                >
                  {saving && <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />}
                  {saving ? "Recording…" : "Record Payment"}
                </button>
              </div>
            </form>
          </div>
        </>
      )}

      <div className="rounded-xl border border-slate-200/70 bg-white shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className="flex flex-col gap-3 border-b border-slate-100 px-5 py-4 sm:flex-row sm:items-center sm:justify-between dark:border-slate-800">
          <h2 className="text-base font-semibold tracking-tight text-slate-900 dark:text-white">Payment History</h2>
          <div className="relative">
            <Icon name="search" className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
            <input
              type="text"
              value={historySearchInput}
              onChange={(e) => setHistorySearchInput(e.target.value)}
              placeholder="Search by member name or number…"
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
                <th className="px-5 py-3 font-medium">Member</th>
                <th className="px-5 py-3 font-medium">Total</th>
                <th className="px-5 py-3 font-medium">Paid</th>
                <th className="px-5 py-3 font-medium">Balance</th>
                <th className="px-5 py-3 font-medium">Next Renewal</th>
                <th className="px-5 py-3 font-medium">Date</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {history === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={7}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {history !== null && history.length === 0 && !historyError && (
                <tr>
                  <td colSpan={7} className="px-5 py-10 text-center text-sm text-slate-400">
                    {historySearchInput ? "No payments match your search." : "No payments recorded yet."}
                  </td>
                </tr>
              )}

              {history?.map((row) => (
                <tr key={row.paymentId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{row.invoiceNo}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                    {row.memberFullName} <span className="font-mono text-xs">({row.memberNo})</span>
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{currency.format(row.totalAmount)}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{currency.format(row.amountPaid)}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                    {row.balanceDue > 0 ? (
                      <span className="font-medium text-amber-600 dark:text-amber-400">{currency.format(row.balanceDue)}</span>
                    ) : (
                      "—"
                    )}
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(row.nextRenewalDate)}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(row.createdOn)}</td>
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
