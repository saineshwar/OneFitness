"use client";

import { useState, type FormEvent } from "react";
import { Modal } from "@/components/Modal";
import { ConfirmDeleteModal } from "@/components/ConfirmDeleteModal";
import { RowActions } from "@/components/RowActions";
import { Field, fieldInputClass, inputBorderClass } from "@/components/Field";
import { useCrudResource } from "@/hooks/useCrudResource";
import { createReason, deleteReason, getReasons, updateReason, type Reason, type ReasonInput } from "@/lib/reason";
import { collectErrors, required, type FieldErrors } from "@/lib/validation";

export default function ReasonsPage() {
  const {
    items: reasons,
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
  } = useCrudResource<Reason, ReasonInput>(
    { list: getReasons, create: createReason, update: updateReason, remove: deleteReason },
    (reason) => reason.reasonId,
  );

  const [formName, setFormName] = useState("");
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  function handleOpenCreate() {
    setFormName("");
    setFieldErrors({});
    openCreate();
  }

  function handleOpenEdit(reason: Reason) {
    setFormName(reason.reasonName);
    setFieldErrors({});
    openEdit(reason);
  }

  function validate(): FieldErrors {
    return collectErrors({ reasonName: required(formName, "Reason name is required.") });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;
    await submit({ reasonName: formName.trim() });
  }

  return (
    <div className="mx-auto max-w-3xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Enquiry Reasons</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
            Manage the reason options offered on the Enquiries form.
          </p>
        </div>
        <button
          type="button"
          onClick={handleOpenCreate}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500"
        >
          <span className="text-base leading-none">+</span>
          Add Reason
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
                <th className="px-5 py-3 font-medium">Reason Name</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {reasons === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={2}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {reasons !== null && reasons.length === 0 && !loadError && (
                <tr>
                  <td colSpan={2} className="px-5 py-10 text-center text-sm text-slate-400">
                    No reasons yet. Click <span className="font-medium text-slate-600 dark:text-slate-300">Add Reason</span>{" "}
                    to create one — this list feeds the Reason dropdown on the Enquiries form.
                  </td>
                </tr>
              )}

              {reasons?.map((reason) => (
                <tr key={reason.reasonId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{reason.reasonName}</td>
                  <td className="px-5 py-3.5">
                    <RowActions
                      label={reason.reasonName}
                      onEdit={() => handleOpenEdit(reason)}
                      onDelete={() => confirmDelete(reason)}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Modal open={modalMode !== null} onClose={closeModal} title={modalMode === "edit" ? "Edit Reason" : "Add Reason"}>
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <Field label="Reason Name" error={fieldErrors.reasonName}>
            <input
              value={formName}
              onChange={(event) => setFormName(event.target.value)}
              autoFocus
              className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.reasonName)}`}
              placeholder="e.g. Walk-in enquiry"
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
        itemLabel={deleteTarget?.reasonName ?? ""}
        deleting={deleting}
        error={deleteError}
        onCancel={cancelDelete}
        onConfirm={handleDelete}
        title="Delete Reason"
      />
    </div>
  );
}
