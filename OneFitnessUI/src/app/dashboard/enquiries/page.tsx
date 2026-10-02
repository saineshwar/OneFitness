"use client";

import { useEffect, useState, type FormEvent } from "react";
import { Modal } from "@/components/Modal";
import { ConfirmDeleteModal } from "@/components/ConfirmDeleteModal";
import { StatusBadge } from "@/components/StatusBadge";
import { ToggleSwitch } from "@/components/ToggleSwitch";
import { RowActions } from "@/components/RowActions";
import { Field, fieldInputClass, inputBorderClass } from "@/components/Field";
import { useCrudResource } from "@/hooks/useCrudResource";
import { createEnquiry, deleteEnquiry, getEnquiries, updateEnquiry, type Enquiry, type EnquiryInput } from "@/lib/enquiry";
import { getWorkOuts, type WorkOut } from "@/lib/workOut";
import { getReasons, type Reason } from "@/lib/reason";
import { collectErrors, matches, required, requiredId, type FieldErrors } from "@/lib/validation";

const genders = [
  { id: 1, label: "Male" },
  { id: 2, label: "Female" },
];

const emptyForm = {
  firstName: "",
  lastName: "",
  middleName: "",
  mobileNo: "",
  emailId: "",
  genderId: 0,
  workOutId: 0,
  reasonId: 0,
  enquiryDetails: "",
  status: true,
};

export default function EnquiriesPage() {
  const {
    items: enquiries,
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
  } = useCrudResource<Enquiry, EnquiryInput>(
    { list: getEnquiries, create: createEnquiry, update: updateEnquiry, remove: deleteEnquiry },
    (enquiry) => enquiry.enquiryId,
  );

  const [workOuts, setWorkOuts] = useState<WorkOut[]>([]);
  const [reasons, setReasons] = useState<Reason[]>([]);

  useEffect(() => {
    getWorkOuts().then(setWorkOuts).catch(() => setWorkOuts([]));
    getReasons().then(setReasons).catch(() => setReasons([]));
  }, []);

  const [form, setForm] = useState(emptyForm);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});

  function updateField<K extends keyof typeof form>(key: K, value: (typeof form)[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function fullName(enquiry: Enquiry) {
    return [enquiry.firstName, enquiry.middleName, enquiry.lastName].filter(Boolean).join(" ");
  }

  function workOutName(id: number | null) {
    return workOuts.find((w) => w.workOutId === id)?.workOutName ?? "—";
  }

  function reasonName(id: number | null) {
    return reasons.find((r) => r.reasonId === id)?.reasonName ?? "—";
  }

  function handleOpenCreate() {
    setForm(emptyForm);
    setFieldErrors({});
    openCreate();
  }

  function handleOpenEdit(enquiry: Enquiry) {
    setForm({
      firstName: enquiry.firstName,
      lastName: enquiry.lastName ?? "",
      middleName: enquiry.middleName ?? "",
      mobileNo: enquiry.mobileNo ?? "",
      emailId: enquiry.emailId ?? "",
      genderId: enquiry.genderId ?? 0,
      workOutId: enquiry.workOutId ?? 0,
      reasonId: enquiry.reasonId ?? 0,
      enquiryDetails: enquiry.enquiryDetails ?? "",
      status: enquiry.status,
    });
    setFieldErrors({});
    openEdit(enquiry);
  }

  function validate(): FieldErrors {
    return collectErrors({
      firstName: required(form.firstName, "First name is required.") ?? matches(form.firstName, /^[a-zA-Z ]*$/, "Letters only."),
      lastName: matches(form.lastName, /^[a-zA-Z ]*$/, "Letters only."),
      middleName: matches(form.middleName, /^[a-zA-Z ]*$/, "Letters only."),
      mobileNo: matches(form.mobileNo, /^[0-9]{10}$/, "Enter a valid 10-digit mobile number."),
      emailId: matches(form.emailId, /^[^\s@]+@[^\s@]+\.[^\s@]+$/, "Enter a valid email address."),
      genderId: requiredId(form.genderId, "Please select a gender."),
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;
    await submit({
      firstName: form.firstName.trim(),
      lastName: form.lastName.trim() || null,
      middleName: form.middleName.trim() || null,
      mobileNo: form.mobileNo.trim() || null,
      emailId: form.emailId.trim() || null,
      genderId: form.genderId,
      workOutId: form.workOutId || null,
      reasonId: form.reasonId || null,
      enquiryDetails: form.enquiryDetails.trim() || null,
      status: form.status,
    });
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Enquiries</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">Track prospective members and follow-up reasons.</p>
        </div>
        <button
          type="button"
          onClick={handleOpenCreate}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500"
        >
          <span className="text-base leading-none">+</span>
          Add Enquiry
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
                <th className="px-5 py-3 font-medium">Name</th>
                <th className="px-5 py-3 font-medium">Mobile</th>
                <th className="px-5 py-3 font-medium">Email</th>
                <th className="px-5 py-3 font-medium">Workout</th>
                <th className="px-5 py-3 font-medium">Reason</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {enquiries === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={7}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {enquiries !== null && enquiries.length === 0 && !loadError && (
                <tr>
                  <td colSpan={7} className="px-5 py-10 text-center text-sm text-slate-400">
                    No enquiries yet. Click <span className="font-medium text-slate-600 dark:text-slate-300">Add Enquiry</span>{" "}
                    to create one.
                  </td>
                </tr>
              )}

              {enquiries?.map((enquiry) => (
                <tr key={enquiry.enquiryId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{fullName(enquiry)}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{enquiry.mobileNo ?? "—"}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{enquiry.emailId ?? "—"}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{workOutName(enquiry.workOutId)}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{reasonName(enquiry.reasonId)}</td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={enquiry.status} />
                  </td>
                  <td className="px-5 py-3.5">
                    <RowActions
                      label={fullName(enquiry)}
                      onEdit={() => handleOpenEdit(enquiry)}
                      onDelete={() => confirmDelete(enquiry)}
                    />
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Modal open={modalMode !== null} onClose={closeModal} title={modalMode === "edit" ? "Edit Enquiry" : "Add Enquiry"} size="lg">
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
            <Field label="First Name" error={fieldErrors.firstName}>
              <input
                value={form.firstName}
                onChange={(e) => updateField("firstName", e.target.value)}
                autoFocus
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.firstName)}`}
                placeholder="Jane"
              />
            </Field>
            <Field label="Middle Name" optional error={fieldErrors.middleName}>
              <input
                value={form.middleName}
                onChange={(e) => updateField("middleName", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.middleName)}`}
              />
            </Field>
            <Field label="Last Name" optional error={fieldErrors.lastName}>
              <input
                value={form.lastName}
                onChange={(e) => updateField("lastName", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.lastName)}`}
              />
            </Field>
            <Field label="Mobile No" optional error={fieldErrors.mobileNo}>
              <input
                value={form.mobileNo}
                onChange={(e) => updateField("mobileNo", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.mobileNo)}`}
                placeholder="9876543210"
              />
            </Field>
            <Field label="Email" optional error={fieldErrors.emailId}>
              <input
                type="email"
                value={form.emailId}
                onChange={(e) => updateField("emailId", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.emailId)}`}
                placeholder="jane@example.com"
              />
            </Field>
            <Field label="Gender" error={fieldErrors.genderId}>
              <select
                value={form.genderId || ""}
                onChange={(e) => updateField("genderId", Number(e.target.value))}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.genderId)}`}
              >
                <option value="" disabled>
                  Select
                </option>
                {genders.map((g) => (
                  <option key={g.id} value={g.id}>
                    {g.label}
                  </option>
                ))}
              </select>
            </Field>
            <Field label="Workout" optional error={fieldErrors.workOutId}>
              <select
                value={form.workOutId || ""}
                onChange={(e) => updateField("workOutId", Number(e.target.value))}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.workOutId)}`}
              >
                <option value="">Select</option>
                {workOuts.map((w) => (
                  <option key={w.workOutId} value={w.workOutId}>
                    {w.workOutName}
                  </option>
                ))}
              </select>
            </Field>
            <Field label="Reason" optional error={fieldErrors.reasonId}>
              <select
                value={form.reasonId || ""}
                onChange={(e) => updateField("reasonId", Number(e.target.value))}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.reasonId)}`}
              >
                <option value="">Select</option>
                {reasons.map((r) => (
                  <option key={r.reasonId} value={r.reasonId}>
                    {r.reasonName}
                  </option>
                ))}
              </select>
              {reasons.length === 0 && (
                <p className="mt-1 text-xs text-amber-600 dark:text-amber-400">
                  No reasons set up yet — add some under{" "}
                  <a href="/dashboard/reasons" className="underline">
                    Enquiry Reasons
                  </a>
                  .
                </p>
              )}
            </Field>
          </div>

          <Field label="Details" optional error={fieldErrors.enquiryDetails}>
            <textarea
              value={form.enquiryDetails}
              onChange={(e) => updateField("enquiryDetails", e.target.value)}
              rows={2}
              maxLength={100}
              className={`${fieldInputClass} resize-none ${inputBorderClass(!!fieldErrors.enquiryDetails)}`}
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
        itemLabel={deleteTarget ? fullName(deleteTarget) : ""}
        deleting={deleting}
        error={deleteError}
        onCancel={cancelDelete}
        onConfirm={handleDelete}
        title="Delete Enquiry"
      />
    </div>
  );
}
