"use client";

import { useState, type FormEvent } from "react";
import { Modal } from "@/components/Modal";
import { ConfirmDeleteModal } from "@/components/ConfirmDeleteModal";
import { StatusBadge } from "@/components/StatusBadge";
import { ToggleSwitch } from "@/components/ToggleSwitch";
import { RowActions } from "@/components/RowActions";
import { Field, fieldInputClass, inputBorderClass } from "@/components/Field";
import { useCrudResource } from "@/hooks/useCrudResource";
import {
  createInstallment,
  deleteInstallment,
  getInstallments,
  updateInstallment,
  type Installment,
  type InstallmentInput,
} from "@/lib/installment";
import { collectErrors, required, type FieldErrors } from "@/lib/validation";

export default function InstallmentPage() {
  const {
    items: installments,
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
  } = useCrudResource<Installment, InstallmentInput>(
    { list: getInstallments, create: createInstallment, update: updateInstallment, remove: deleteInstallment },
    (installment) => installment.installmentId,
  );

  const [formName, setFormName] = useState("");
  const [formMonths, setFormMonths] = useState("");
  const [formStatus, setFormStatus] = useState(true);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  function handleOpenCreate() {
    setFormName("");
    setFormMonths("");
    setFormStatus(true);
    setFieldErrors({});
    openCreate();
  }

  function handleOpenEdit(installment: Installment) {
    setFormName(installment.installmentName);
    setFormMonths(installment.installmentMonths?.toString() ?? "");
    setFormStatus(installment.status);
    setFieldErrors({});
    openEdit(installment);
  }

  function validate(): FieldErrors {
    return collectErrors({
      installmentName: required(formName, "Installment name is required."),
      installmentMonths:
        formMonths.trim() && Number(formMonths) <= 0 ? "Months must be a positive number." : undefined,
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;
    await submit({
      installmentName: formName.trim(),
      installmentMonths: formMonths.trim() ? Number(formMonths) : null,
      status: formStatus,
    });
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Installments</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
            Manage the installment plans available for membership payments.
          </p>
        </div>
        <button
          type="button"
          onClick={handleOpenCreate}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500"
        >
          <span className="text-base leading-none">+</span>
          Add Installment
        </button>
      </div>

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
                <th className="px-5 py-3 font-medium">Installment Name</th>
                <th className="px-5 py-3 font-medium">Months</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {installments === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={4}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {installments !== null && installments.length === 0 && !loadError && (
                <tr>
                  <td colSpan={4} className="px-5 py-10 text-center text-sm text-slate-400">
                    No installment plans yet. Click{" "}
                    <span className="font-medium text-slate-600 dark:text-slate-300">Add Installment</span> to create one.
                  </td>
                </tr>
              )}

              {installments?.map((installment) => (
                <tr key={installment.installmentId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">
                    {installment.installmentName}
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">
                    {installment.installmentMonths ?? "—"}
                  </td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={installment.status} />
                  </td>
                  <td className="px-5 py-3.5">
                    <RowActions
                      label={installment.installmentName}
                      onEdit={() => handleOpenEdit(installment)}
                      onDelete={() => confirmDelete(installment)}
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
        title={modalMode === "edit" ? "Edit Installment" : "Add Installment"}
      >
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <Field label="Installment Name" error={fieldErrors.installmentName}>
            <input
              value={formName}
              onChange={(event) => setFormName(event.target.value)}
              autoFocus
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.installmentName)}`}
              placeholder="e.g. Quarterly"
            />
          </Field>

          <Field label="Months" optional error={fieldErrors.installmentMonths}>
            <input
              type="number"
              min={1}
              value={formMonths}
              onChange={(event) => setFormMonths(event.target.value)}
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.installmentMonths)}`}
              placeholder="e.g. 3"
            />
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
        itemLabel={deleteTarget?.installmentName ?? ""}
        deleting={deleting}
        error={deleteError}
        onCancel={cancelDelete}
        onConfirm={handleDelete}
        title="Delete Installment"
      />
    </div>
  );
}
