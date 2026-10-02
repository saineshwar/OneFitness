"use client";

import { useEffect, useState, type FormEvent } from "react";
import { Modal } from "@/components/Modal";
import { ConfirmDeleteModal } from "@/components/ConfirmDeleteModal";
import { StatusBadge } from "@/components/StatusBadge";
import { ToggleSwitch } from "@/components/ToggleSwitch";
import { RowActions } from "@/components/RowActions";
import { Field, fieldInputClass, inputBorderClass } from "@/components/Field";
import { useCrudResource } from "@/hooks/useCrudResource";
import {
  createMembershipType,
  deleteMembershipType,
  getMembershipTypes,
  updateMembershipType,
  type MembershipType,
  type MembershipTypeInput,
} from "@/lib/membershipType";
import { getWorkOuts, type WorkOut } from "@/lib/workOut";
import { getInstallments, type Installment } from "@/lib/installment";
import { collectErrors, required, type FieldErrors } from "@/lib/validation";

const currency = new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR", maximumFractionDigits: 0 });

export default function MembershipTypePage() {
  const {
    items: membershipTypes,
    loadError,
    reload,
    modalMode,
    openCreate,
    openEdit,
    closeModal,
    saving,
    formError,
    submit,
    deleteTarget,
    deleting,
    deleteError,
    confirmDelete,
    cancelDelete,
    handleDelete,
  } = useCrudResource<MembershipType, MembershipTypeInput>(
    {
      list: getMembershipTypes,
      create: createMembershipType,
      update: updateMembershipType,
      remove: deleteMembershipType,
    },
    (membershipType) => membershipType.membershipTypeId,
  );

  const [workOuts, setWorkOuts] = useState<WorkOut[]>([]);
  const [installments, setInstallments] = useState<Installment[]>([]);

  useEffect(() => {
    getWorkOuts().then(setWorkOuts).catch(() => setWorkOuts([]));
    getInstallments().then(setInstallments).catch(() => setInstallments([]));
  }, []);

  const [formName, setFormName] = useState("");
  const [formAmount, setFormAmount] = useState("");
  const [formWorkOutId, setFormWorkOutId] = useState("");
  const [formInstallmentId, setFormInstallmentId] = useState("");
  const [formStatus, setFormStatus] = useState(true);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  function workOutName(id: number | null) {
    return workOuts.find((w) => w.workOutId === id)?.workOutName ?? "—";
  }

  function installmentName(id: number | null) {
    return installments.find((i) => i.installmentId === id)?.installmentName ?? "—";
  }

  function handleOpenCreate() {
    setFormName("");
    setFormAmount("");
    setFormWorkOutId(workOuts[0] ? String(workOuts[0].workOutId) : "");
    setFormInstallmentId(installments[0] ? String(installments[0].installmentId) : "");
    setFormStatus(true);
    setFieldErrors({});
    openCreate();
  }

  function handleOpenEdit(membershipType: MembershipType) {
    setFormName(membershipType.membershipTypeName);
    setFormAmount(String(membershipType.amount));
    setFormWorkOutId(membershipType.workOutId ? String(membershipType.workOutId) : "");
    setFormInstallmentId(membershipType.installmentId ? String(membershipType.installmentId) : "");
    setFormStatus(membershipType.status);
    setFieldErrors({});
    openEdit(membershipType);
  }

  function validate(): FieldErrors {
    return collectErrors({
      membershipTypeName: required(formName, "Name is required."),
      amount: !formAmount.trim()
        ? "Amount is required."
        : Number(formAmount) <= 0
          ? "Amount must be greater than 0."
          : undefined,
      workOutId: !formWorkOutId ? "Please select a workout." : undefined,
      installmentId: !formInstallmentId ? "Please select an installment plan." : undefined,
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;
    await submit({
      membershipTypeName: formName.trim(),
      amount: Number(formAmount),
      workOutId: Number(formWorkOutId),
      installmentId: Number(formInstallmentId),
      status: formStatus,
    });
  }

  const noReferenceData = workOuts.length === 0 || installments.length === 0;

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Membership Types</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
            Manage the membership plans members can sign up for.
          </p>
        </div>
        <button
          type="button"
          onClick={handleOpenCreate}
          disabled={noReferenceData}
          title={noReferenceData ? "Add a workout and an installment plan first" : undefined}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500 disabled:cursor-not-allowed disabled:opacity-50"
        >
          <span className="text-base leading-none">+</span>
          Add Membership Type
        </button>
      </div>

      {noReferenceData && (
        <div className="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-700 dark:border-amber-900 dark:bg-amber-950 dark:text-amber-300">
          Membership types require at least one Workout and one Installment plan. Create those first.
        </div>
      )}

      {loadError && (
        <div className="flex items-center justify-between gap-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300">
          <span>{loadError}</span>
          <button
            type="button"
            onClick={reload}
            className="shrink-0 rounded-md bg-white px-2.5 py-1 text-xs font-medium text-red-700 shadow-sm transition hover:bg-red-100 dark:bg-slate-900 dark:hover:bg-slate-800"
          >
            Retry
          </button>
        </div>
      )}

      <div className="rounded-xl border border-slate-200/70 bg-white shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className="overflow-x-auto">
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
                <th className="px-5 py-3 font-medium">Name</th>
                <th className="px-5 py-3 font-medium">Amount</th>
                <th className="px-5 py-3 font-medium">Workout</th>
                <th className="px-5 py-3 font-medium">Installment</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {membershipTypes === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={6}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {membershipTypes !== null && membershipTypes.length === 0 && !loadError && (
                <tr>
                  <td colSpan={6} className="px-5 py-10 text-center text-sm text-slate-400">
                    No membership types yet.
                  </td>
                </tr>
              )}

              {membershipTypes?.map((membershipType) => (
                <tr
                  key={membershipType.membershipTypeId}
                  className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50"
                >
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">
                    {membershipType.membershipTypeName}
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                    {currency.format(membershipType.amount)}
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                    {workOutName(membershipType.workOutId)}
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                    {installmentName(membershipType.installmentId)}
                  </td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={membershipType.status} />
                  </td>
                  <td className="px-5 py-3.5">
                    <RowActions
                      label={membershipType.membershipTypeName}
                      onEdit={() => handleOpenEdit(membershipType)}
                      onDelete={() => confirmDelete(membershipType)}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Modal
        open={modalMode !== null}
        onClose={closeModal}
        title={modalMode === "edit" ? "Edit Membership Type" : "Add Membership Type"}
      >
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <Field label="Name" error={fieldErrors.membershipTypeName}>
            <input
              value={formName}
              onChange={(event) => setFormName(event.target.value)}
              autoFocus
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.membershipTypeName)}`}
              placeholder="e.g. Gold - 12 Months"
            />
          </Field>

          <Field label="Amount" error={fieldErrors.amount}>
            <input
              type="number"
              min={0}
              step="0.01"
              value={formAmount}
              onChange={(event) => setFormAmount(event.target.value)}
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.amount)}`}
              placeholder="e.g. 15000"
            />
          </Field>

          <Field label="Workout" error={fieldErrors.workOutId}>
            <select
              value={formWorkOutId}
              onChange={(event) => setFormWorkOutId(event.target.value)}
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.workOutId)}`}
            >
              <option value="" disabled>
                Select a workout
              </option>
              {workOuts.map((workOut) => (
                <option key={workOut.workOutId} value={workOut.workOutId}>
                  {workOut.workOutName}
                </option>
              ))}
            </select>
          </Field>

          <Field label="Installment Plan" error={fieldErrors.installmentId}>
            <select
              value={formInstallmentId}
              onChange={(event) => setFormInstallmentId(event.target.value)}
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.installmentId)}`}
            >
              <option value="" disabled>
                Select an installment plan
              </option>
              {installments.map((installment) => (
                <option key={installment.installmentId} value={installment.installmentId}>
                  {installment.installmentName}
                </option>
              ))}
            </select>
          </Field>

          <ToggleSwitch label="Active" checked={formStatus} onChange={setFormStatus} />

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
        itemLabel={deleteTarget?.membershipTypeName ?? ""}
        deleting={deleting}
        error={deleteError}
        onCancel={cancelDelete}
        onConfirm={handleDelete}
        title="Delete Membership Type"
      />
    </div>
  );
}
