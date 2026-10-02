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
  createTaxMaster,
  deleteTaxMaster,
  getTaxMasters,
  updateTaxMaster,
  type TaxMaster,
  type TaxMasterInput,
} from "@/lib/taxMaster";
import { collectErrors, required, type FieldErrors } from "@/lib/validation";

export default function TaxMasterPage() {
  const {
    items: taxMasters,
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
  } = useCrudResource<TaxMaster, TaxMasterInput>(
    { list: getTaxMasters, create: createTaxMaster, update: updateTaxMaster, remove: deleteTaxMaster },
    (taxMaster) => taxMaster.taxId,
  );

  const [formTaxType, setFormTaxType] = useState("");
  const [formTaxRate, setFormTaxRate] = useState("");
  const [formIdentificationNo, setFormIdentificationNo] = useState("");
  const [formStatus, setFormStatus] = useState(true);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  function handleOpenCreate() {
    setFormTaxType("");
    setFormTaxRate("");
    setFormIdentificationNo("");
    setFormStatus(true);
    setFieldErrors({});
    openCreate();
  }

  function handleOpenEdit(taxMaster: TaxMaster) {
    setFormTaxType(taxMaster.taxType);
    setFormTaxRate(taxMaster.taxRate.toString());
    setFormIdentificationNo(taxMaster.identificationNo ?? "");
    setFormStatus(taxMaster.status);
    setFieldErrors({});
    openEdit(taxMaster);
  }

  function validate(): FieldErrors {
    return collectErrors({
      taxType: required(formTaxType, "Tax type is required."),
      taxRate:
        required(formTaxRate, "Tax rate is required.") ??
        (Number(formTaxRate) < 0 ? "Tax rate cannot be negative." : undefined),
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;
    await submit({
      taxType: formTaxType.trim(),
      taxRate: Number(formTaxRate),
      status: formStatus,
      identificationNo: formIdentificationNo.trim() || null,
    });
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Tax Master</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
            Manage the tax rates available for membership payments and receipts.
          </p>
        </div>
        <button
          type="button"
          onClick={handleOpenCreate}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500"
        >
          <span className="text-base leading-none">+</span>
          Add Tax
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
                <th className="px-5 py-3 font-medium">Tax Type</th>
                <th className="px-5 py-3 font-medium">Rate</th>
                <th className="px-5 py-3 font-medium">Identification No</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {taxMasters === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={5}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {taxMasters !== null && taxMasters.length === 0 && !loadError && (
                <tr>
                  <td colSpan={5} className="px-5 py-10 text-center text-sm text-slate-400">
                    No tax rates yet. Click <span className="font-medium text-slate-600 dark:text-slate-300">Add Tax</span> to
                    create one.
                  </td>
                </tr>
              )}

              {taxMasters?.map((taxMaster) => (
                <tr key={taxMaster.taxId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{taxMaster.taxType}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{taxMaster.taxRate}%</td>
                  <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">
                    {taxMaster.identificationNo ?? "—"}
                  </td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={taxMaster.status} />
                  </td>
                  <td className="px-5 py-3.5">
                    <RowActions
                      label={taxMaster.taxType}
                      onEdit={() => handleOpenEdit(taxMaster)}
                      onDelete={() => confirmDelete(taxMaster)}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Modal open={modalMode !== null} onClose={closeModal} title={modalMode === "edit" ? "Edit Tax" : "Add Tax"}>
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <Field label="Tax Type" error={fieldErrors.taxType}>
            <input
              value={formTaxType}
              onChange={(event) => setFormTaxType(event.target.value)}
              autoFocus
              maxLength={10}
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.taxType)}`}
              placeholder="e.g. GST"
            />
          </Field>

          <Field label="Tax Rate (%)" error={fieldErrors.taxRate}>
            <input
              type="number"
              min={0}
              step="0.01"
              value={formTaxRate}
              onChange={(event) => setFormTaxRate(event.target.value)}
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.taxRate)}`}
              placeholder="e.g. 18"
            />
          </Field>

          <Field label="Identification No" optional error={fieldErrors.identificationNo}>
            <input
              value={formIdentificationNo}
              onChange={(event) => setFormIdentificationNo(event.target.value)}
              maxLength={50}
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.identificationNo)}`}
              placeholder="e.g. GSTIN"
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
        itemLabel={deleteTarget?.taxType ?? ""}
        deleting={deleting}
        error={deleteError}
        onCancel={cancelDelete}
        onConfirm={handleDelete}
        title="Delete Tax"
      />
    </div>
  );
}
