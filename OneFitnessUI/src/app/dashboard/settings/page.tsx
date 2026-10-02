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
  createGeneralSettings,
  deleteGeneralSettings,
  getGeneralSettingsList,
  updateGeneralSettings,
  type GeneralSettings,
  type GeneralSettingsInput,
} from "@/lib/generalSettings";
import { collectErrors, matches, patterns, required, type FieldErrors } from "@/lib/validation";

const emptyForm: GeneralSettingsInput = {
  name: "",
  supportEmailId: "",
  websiteTitle: "",
  websiteUrl: "",
  telephoneNo: "",
  mobileNo: "",
  status: true,
  logopath: "",
  logoFileName: "",
  address: "",
};

export default function GeneralSettingsPage() {
  const {
    items: settingsList,
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
  } = useCrudResource<GeneralSettings, GeneralSettingsInput>(
    {
      list: getGeneralSettingsList,
      create: createGeneralSettings,
      update: updateGeneralSettings,
      remove: deleteGeneralSettings,
    },
    (settings) => settings.companyId,
  );

  const [form, setForm] = useState<GeneralSettingsInput>(emptyForm);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  function updateField<K extends keyof GeneralSettingsInput>(key: K, value: GeneralSettingsInput[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function handleOpenCreate() {
    setForm(emptyForm);
    setFieldErrors({});
    openCreate();
  }

  function handleOpenEdit(settings: GeneralSettings) {
    setForm({
      name: settings.name,
      supportEmailId: settings.supportEmailId,
      websiteTitle: settings.websiteTitle,
      websiteUrl: settings.websiteUrl ?? "",
      telephoneNo: settings.telephoneNo ?? "",
      mobileNo: settings.mobileNo ?? "",
      status: settings.status,
      logopath: settings.logopath ?? "",
      logoFileName: settings.logoFileName ?? "",
      address: settings.address,
    });
    setFieldErrors({});
    openEdit(settings);
  }

  function validate(): FieldErrors {
    return collectErrors({
      name: required(form.name, "Company name is required."),
      supportEmailId:
        required(form.supportEmailId, "Support email is required.") ??
        matches(form.supportEmailId, patterns.email, "Enter a valid email address."),
      websiteTitle: required(form.websiteTitle, "Website title is required."),
      address: required(form.address, "Address is required."),
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;
    await submit(form);
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">General Settings</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
            Manage company profile and site details shown across the portal.
          </p>
        </div>
        <button
          type="button"
          onClick={handleOpenCreate}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500"
        >
          <span className="text-base leading-none">+</span>
          Add Settings
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
                <th className="px-5 py-3 font-medium">Company Name</th>
                <th className="px-5 py-3 font-medium">Support Email</th>
                <th className="px-5 py-3 font-medium">Website</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {settingsList === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={5}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {settingsList !== null && settingsList.length === 0 && !loadError && (
                <tr>
                  <td colSpan={5} className="px-5 py-10 text-center text-sm text-slate-400">
                    No settings configured yet. Click{" "}
                    <span className="font-medium text-slate-600 dark:text-slate-300">Add Settings</span> to create one.
                  </td>
                </tr>
              )}

              {settingsList?.map((settings) => (
                <tr key={settings.companyId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{settings.name}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{settings.supportEmailId}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{settings.websiteTitle}</td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={settings.status} />
                  </td>
                  <td className="px-5 py-3.5">
                    <RowActions
                      label={settings.name}
                      onEdit={() => handleOpenEdit(settings)}
                      onDelete={() => confirmDelete(settings)}
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
        title={modalMode === "edit" ? "Edit Settings" : "Add Settings"}
        size="lg"
      >
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field label="Company Name" error={fieldErrors.name}>
              <input
                value={form.name}
                onChange={(event) => updateField("name", event.target.value)}
                autoFocus
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.name)}`}
                placeholder="e.g. OneFitness Gym"
              />
            </Field>

            <Field label="Support Email" error={fieldErrors.supportEmailId}>
              <input
                type="email"
                value={form.supportEmailId}
                onChange={(event) => updateField("supportEmailId", event.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.supportEmailId)}`}
                placeholder="support@example.com"
              />
            </Field>

            <Field label="Mobile No" optional>
              <input
                value={form.mobileNo}
                onChange={(event) => updateField("mobileNo", event.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(false)}`}
                placeholder="9876543210"
              />
            </Field>

            <Field label="Telephone No" optional>
              <input
                value={form.telephoneNo}
                onChange={(event) => updateField("telephoneNo", event.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(false)}`}
                placeholder="022-12345678"
              />
            </Field>

            <Field label="Website Title" error={fieldErrors.websiteTitle}>
              <input
                value={form.websiteTitle}
                onChange={(event) => updateField("websiteTitle", event.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.websiteTitle)}`}
                placeholder="e.g. OneFitness"
              />
            </Field>

            <Field label="Website URL" optional>
              <input
                value={form.websiteUrl}
                onChange={(event) => updateField("websiteUrl", event.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(false)}`}
                placeholder="https://example.com"
              />
            </Field>
          </div>

          <Field label="Address" error={fieldErrors.address}>
            <textarea
              value={form.address}
              onChange={(event) => updateField("address", event.target.value)}
              rows={3}
              maxLength={200}
              className={`${fieldInputClass} resize-none ${inputBorderClass(!!fieldErrors.address)}`}
              placeholder="Registered business address"
            />
          </Field>

          <ToggleSwitch label="Active" checked={form.status} onChange={(value) => updateField("status", value)} />

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
        itemLabel={deleteTarget?.name ?? ""}
        deleting={deleting}
        error={deleteError}
        onCancel={cancelDelete}
        onConfirm={handleDelete}
        title="Delete Settings"
      />
    </div>
  );
}
