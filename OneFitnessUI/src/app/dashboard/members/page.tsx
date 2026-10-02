"use client";

import { useEffect, useRef, useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import { Modal } from "@/components/Modal";
import { ConfirmDeleteModal } from "@/components/ConfirmDeleteModal";
import { StatusBadge } from "@/components/StatusBadge";
import { ToggleSwitch } from "@/components/ToggleSwitch";
import { RowActions } from "@/components/RowActions";
import { Field, fieldInputClass, inputBorderClass } from "@/components/Field";
import { Icon } from "@/components/icons";
import { Pagination } from "@/components/Pagination";
import { PhotoCapture } from "@/components/PhotoCapture";
import { useCrudResourcePaged } from "@/hooks/useCrudResourcePaged";
import {
  createMember,
  deleteMember,
  deleteMemberPhoto,
  getMemberPhoto,
  getMembersPaged,
  saveMemberPhoto,
  updateMember,
  type Member,
  type MemberInput,
} from "@/lib/member";
import { getMembershipTypes, type MembershipType } from "@/lib/membershipType";
import { getReceiptHistory, type ReceiptHistory } from "@/lib/receipt";
import { getLatestPaymentByMember, type Payment } from "@/lib/payment";
import { ApiError } from "@/lib/api";
import { collectErrors, matches, patterns, required, requiredId, type FieldErrors } from "@/lib/validation";

const genders = [
  { id: 1, label: "Male" },
  { id: 2, label: "Female" },
];

function toDateInputValue(value: string | null) {
  return value ? value.slice(0, 10) : "";
}

function formatDate(value: string | null) {
  if (!value) return "—";
  return new Date(value).toLocaleDateString("en-IN", { year: "numeric", month: "short", day: "numeric" });
}

function calculateAge(dob: string) {
  if (!dob) return "";
  const dobYear = new Date(dob).getFullYear();
  if (Number.isNaN(dobYear)) return "";
  return String(new Date().getFullYear() - dobYear);
}

const emptyForm: Omit<MemberInput, "age"> & { age: string } = {
  firstName: "",
  lastName: "",
  middleName: "",
  dob: "",
  age: "",
  mobileNo: "",
  emailId: "",
  genderId: 0,
  address: "",
  joiningDate: new Date().toISOString().slice(0, 10),
  emergencyContactName: "",
  emergencyContactNo: "",
  status: true,
};

export default function MemberPage() {
  const router = useRouter();
  const {
    items: members,
    loading: membersLoading,
    loadError,
    reload,
    page,
    setPage,
    totalPages,
    totalCount,
    pageSize,
    searchInput,
    setSearchInput,
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
  } = useCrudResourcePaged<Member, MemberInput>(
    { list: getMembersPaged, create: createMember, update: updateMember, remove: deleteMember },
    (member) => member.memberId,
  );

  const [membershipTypes, setMembershipTypes] = useState<MembershipType[]>([]);

  useEffect(() => {
    getMembershipTypes().then(setMembershipTypes).catch(() => setMembershipTypes([]));
  }, []);

  const [form, setForm] = useState(emptyForm);
  const [memberNo, setMemberNo] = useState<string | null>(null);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [justCreated, setJustCreated] = useState<Member | null>(null);

  // Photo is saved through its own endpoint after the member itself is saved; it is only sent when it changed.
  const [photo, setPhoto] = useState<string | null>(null);
  const [originalPhoto, setOriginalPhoto] = useState<string | null>(null);
  const [photoLoading, setPhotoLoading] = useState(false);
  const [photoError, setPhotoError] = useState<string | null>(null);
  const photoRequestRef = useRef(0);

  function resetPhoto() {
    photoRequestRef.current++;
    setPhoto(null);
    setOriginalPhoto(null);
    setPhotoLoading(false);
  }

  async function loadPhoto(memberId: number) {
    const requestId = ++photoRequestRef.current;
    setPhotoLoading(true);
    try {
      const existing = await getMemberPhoto(memberId);
      if (requestId !== photoRequestRef.current) return;
      setPhoto(existing);
      setOriginalPhoto(existing);
    } catch {
      // Leave the photo empty; the rest of the form is still editable.
    } finally {
      if (requestId === photoRequestRef.current) setPhotoLoading(false);
    }
  }

  async function persistPhoto(memberId: number) {
    if (photo === originalPhoto) return;
    try {
      if (photo) {
        await saveMemberPhoto(memberId, photo);
      } else {
        await deleteMemberPhoto(memberId);
      }
    } catch (error) {
      setPhotoError(
        `Member was saved, but the photo could not be ${photo ? "saved" : "removed"}: ${
          error instanceof ApiError ? error.message : "unknown error"
        }`,
      );
    }
  }

  function updateField<K extends keyof typeof form>(key: K, value: (typeof form)[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function fullName(member: Member) {
    return [member.firstName, member.middleName, member.lastName].filter(Boolean).join(" ");
  }

  const [historyTarget, setHistoryTarget] = useState<Member | null>(null);
  const [historyRows, setHistoryRows] = useState<ReceiptHistory[] | null>(null);
  const [historyError, setHistoryError] = useState<string | null>(null);
  const [historyPayment, setHistoryPayment] = useState<Payment | null>(null);

  async function openHistory(member: Member) {
    setHistoryTarget(member);
    setHistoryRows(null);
    setHistoryError(null);
    setHistoryPayment(null);
    try {
      const all = await getReceiptHistory();
      setHistoryRows(all.filter((row) => row.memberNo === member.memberNo));
    } catch (error) {
      setHistoryRows([]);
      setHistoryError(error instanceof ApiError ? error.message : "Failed to load membership history.");
    }
    try {
      setHistoryPayment(await getLatestPaymentByMember(member.memberId));
    } catch {
      setHistoryPayment(null);
    }
  }

  function closeHistory() {
    setHistoryTarget(null);
    setHistoryRows(null);
    setHistoryError(null);
    setHistoryPayment(null);
  }

  function formatDateTime(value: string) {
    return new Date(value).toLocaleString("en-IN", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "numeric",
      minute: "2-digit",
    });
  }

  function handleOpenCreate() {
    setForm(emptyForm);
    setMemberNo(null);
    setFieldErrors({});
    setJustCreated(null);
    setPhotoError(null);
    resetPhoto();
    openCreate();
  }

  function handleOpenEdit(member: Member) {
    setForm({
      firstName: member.firstName,
      lastName: member.lastName ?? "",
      middleName: member.middleName ?? "",
      dob: toDateInputValue(member.dob),
      age: member.age?.toString() ?? "",
      mobileNo: member.mobileNo ?? "",
      emailId: member.emailId ?? "",
      genderId: member.genderId ?? 0,
      address: member.address,
      joiningDate: toDateInputValue(member.joiningDate),
      emergencyContactName: member.emergencyContactName ?? "",
      emergencyContactNo: member.emergencyContactNo ?? "",
      status: member.status,
    });
    setMemberNo(member.memberNo);
    setFieldErrors({});
    setPhotoError(null);
    resetPhoto();
    loadPhoto(member.memberId);
    openEdit(member);
  }

  function handleDobChange(value: string) {
    setForm((current) => ({ ...current, dob: value, age: calculateAge(value) }));
  }

  function validate(): FieldErrors {
    return collectErrors({
      firstName:
        required(form.firstName, "First name is required.") ??
        matches(form.firstName, patterns.name, "Enter a valid first name (letters only)."),
      middleName: matches(form.middleName, patterns.name, "Enter a valid middle name (letters only)."),
      lastName: matches(form.lastName, patterns.name, "Enter a valid last name (letters only)."),
      dob: required(form.dob, "Date of birth is required."),
      age: required(form.age, "Age is required."),
      emailId: matches(form.emailId, patterns.email, "Enter a valid email address."),
      genderId: requiredId(form.genderId, "Please select a gender."),
      address: required(form.address, "Address is required."),
      joiningDate: required(form.joiningDate, "Joining date is required."),
      emergencyContactName: matches(form.emergencyContactName, patterns.name, "Enter a valid name (letters only)."),
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;
    const wasCreate = modalMode === "create";
    const result = await submit({
      ...form,
      lastName: form.lastName.trim(),
      middleName: form.middleName.trim(),
      age: Number(form.age),
    });
    if (result) {
      await persistPhoto(result.memberId);
    }
    if (result && wasCreate) {
      setJustCreated(result);
    }
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Members</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">
            Manage gym member registrations. Payments are recorded separately on the Payments page.
          </p>
        </div>
        <button
          type="button"
          onClick={handleOpenCreate}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500"
        >
          <span className="text-base leading-none">+</span>
          Add Member
        </button>
      </div>

      {justCreated && (
        <div className="flex items-start justify-between gap-4 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700 dark:border-emerald-900 dark:bg-emerald-950 dark:text-emerald-300">
          <div className="flex items-start gap-3">
            <Icon name="check" className="mt-0.5 h-4 w-4 shrink-0" />
            <p>
              <strong>{fullName(justCreated)}</strong> was added. Record their first payment to activate a membership
              plan.
            </p>
          </div>
          <button
            type="button"
            onClick={() => router.push(`/dashboard/payments?memberId=${justCreated.memberId}`)}
            className="shrink-0 rounded-md bg-white px-2.5 py-1 text-xs font-medium text-emerald-700 shadow-sm transition hover:bg-emerald-100 dark:bg-slate-900 dark:hover:bg-slate-800"
          >
            Record Payment
          </button>
        </div>
      )}

      {photoError && (
        <div className="flex items-center justify-between gap-4 rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800 dark:border-amber-900 dark:bg-amber-950 dark:text-amber-300">
          <span>{photoError}</span>
          <button
            type="button"
            onClick={() => setPhotoError(null)}
            className="shrink-0 rounded-md bg-white px-2.5 py-1 text-xs font-medium text-amber-800 shadow-sm transition hover:bg-amber-100 dark:bg-slate-900 dark:hover:bg-slate-800"
          >
            Dismiss
          </button>
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

      <div className="relative">
        <Icon name="search" className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
        <input
          type="text"
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
          placeholder="Search by name, member no, mobile or email…"
          className="w-full max-w-sm rounded-lg border border-slate-300 bg-white py-2.5 pr-3 pl-9 text-sm text-slate-900 outline-none transition focus:border-indigo-500 focus:ring-4 focus:ring-indigo-500/10 dark:border-slate-700 dark:bg-slate-900 dark:text-white"
        />
      </div>

      <div className="rounded-xl border border-slate-200/70 bg-white shadow-sm dark:border-slate-800 dark:bg-slate-900">
        <div className={`overflow-x-auto transition-opacity ${membersLoading && members !== null ? "opacity-60" : ""}`}>
          <table className="w-full text-left text-sm">
            <thead>
              <tr className="border-b border-slate-100 text-xs tracking-wide text-slate-400 uppercase dark:border-slate-800">
                <th className="px-5 py-3 font-medium">Member No</th>
                <th className="px-5 py-3 font-medium">Name</th>
                <th className="px-5 py-3 font-medium">Mobile</th>
                <th className="px-5 py-3 font-medium">Joining Date</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {members === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={6}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {members !== null && members.length === 0 && !loadError && (
                <tr>
                  <td colSpan={6} className="px-5 py-10 text-center text-sm text-slate-400">
                    {searchInput ? "No members match your search." : "No members yet."}
                  </td>
                </tr>
              )}

              {members?.map((member) => (
                <tr key={member.memberId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{member.memberNo}</td>
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{fullName(member)}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{member.mobileNo ?? "—"}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(member.joiningDate)}</td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={member.status} />
                  </td>
                  <td className="px-5 py-3.5">
                    <div className="flex items-center justify-end gap-1">
                      <button
                        type="button"
                        onClick={() => openHistory(member)}
                        aria-label={`View membership history for ${fullName(member)}`}
                        title="Membership history"
                        className="rounded-lg p-2 text-slate-400 transition hover:bg-slate-100 hover:text-indigo-600 dark:hover:bg-slate-800 dark:hover:text-indigo-300"
                      >
                        <Icon name="history" className="h-4 w-4" />
                      </button>
                      <RowActions
                        label={fullName(member)}
                        onEdit={() => handleOpenEdit(member)}
                        onDelete={() => confirmDelete(member)}
                      />
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <Pagination page={page} totalPages={totalPages} totalCount={totalCount} pageSize={pageSize} onPageChange={setPage} />
      </div>

      <Modal
        open={modalMode !== null}
        onClose={closeModal}
        title={modalMode === "edit" ? "Edit Member" : "Add Member"}
        size="2xl"
      >
        <form onSubmit={handleSubmit} noValidate className="space-y-3">
          {memberNo && (
            <div className="rounded-lg bg-slate-50 px-3 py-2 text-xs text-slate-500 dark:bg-slate-800 dark:text-slate-400">
              Member No: <span className="font-mono text-slate-700 dark:text-slate-300">{memberNo}</span>
            </div>
          )}

          <PhotoCapture value={photo} onChange={setPhoto} loading={photoLoading} />

          <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
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

            <Field label="Date of Birth" error={fieldErrors.dob}>
              <input
                type="date"
                value={form.dob}
                onChange={(e) => handleDobChange(e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.dob)}`}
              />
            </Field>
            <Field label="Age" error={fieldErrors.age}>
              <input
                value={form.age}
                readOnly
                disabled
                className={`${fieldInputClass} bg-slate-50 dark:bg-slate-800 ${inputBorderClass(!!fieldErrors.age)}`}
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

            <Field label="Joining Date" error={fieldErrors.joiningDate}>
              <input
                type="date"
                value={form.joiningDate}
                onChange={(e) => updateField("joiningDate", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.joiningDate)}`}
              />
            </Field>
            <Field label="Emergency Contact Name" optional error={fieldErrors.emergencyContactName}>
              <input
                value={form.emergencyContactName}
                onChange={(e) => updateField("emergencyContactName", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.emergencyContactName)}`}
              />
            </Field>
            <Field label="Emergency Contact No" optional error={fieldErrors.emergencyContactNo}>
              <input
                value={form.emergencyContactNo}
                onChange={(e) => updateField("emergencyContactNo", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.emergencyContactNo)}`}
              />
            </Field>
          </div>

          <Field label="Address" error={fieldErrors.address}>
            <textarea
              value={form.address}
              onChange={(e) => updateField("address", e.target.value)}
              rows={2}
              maxLength={200}
              className={`${fieldInputClass} resize-none ${inputBorderClass(!!fieldErrors.address)}`}
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
        title="Delete Member"
      />

      <Modal
        open={historyTarget !== null}
        onClose={closeHistory}
        title={historyTarget ? `Membership History — ${fullName(historyTarget)}` : "Membership History"}
        size="lg"
      >
        {historyTarget && (
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-3 rounded-lg border border-slate-200 bg-slate-50 px-4 py-3 text-sm sm:grid-cols-4 dark:border-slate-800 dark:bg-slate-800/50">
              <SummaryItem
                label="Current Plan"
                value={
                  historyPayment
                    ? (membershipTypes.find((m) => m.membershipTypeId === historyPayment.membershipTypeId)
                        ?.membershipTypeName ?? "—")
                    : "No payment yet"
                }
              />
              <SummaryItem
                label="Next Renewal"
                value={historyPayment ? toDateInputValue(historyPayment.nextRenewalDate) : "—"}
              />
              <SummaryItem label="Current Invoice No" value={historyPayment?.invoiceNo?.toString() ?? "—"} />
              <SummaryItem label="Status" value={historyTarget.status ? "Active" : "Inactive"} />
            </div>

            <p className="text-xs text-slate-400">
              Every invoice issued to this member — at joining and at each renewal, plus any manually generated
              receipt.
            </p>

            {historyError && (
              <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700 dark:bg-red-950 dark:text-red-300">
                {historyError}
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
                  {historyRows === null &&
                    [0, 1].map((i) => (
                      <tr key={i}>
                        <td className="px-4 py-3" colSpan={3}>
                          <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                        </td>
                      </tr>
                    ))}

                  {historyRows !== null && historyRows.length === 0 && !historyError && (
                    <tr>
                      <td colSpan={3} className="px-4 py-8 text-center text-sm text-slate-400">
                        No receipts generated for this member yet.
                      </td>
                    </tr>
                  )}

                  {historyRows?.map((row) => (
                    <tr key={row.receiptHistoryId}>
                      <td className="px-4 py-3 font-medium text-slate-800 dark:text-slate-200">{row.invoiceNo ?? "—"}</td>
                      <td className="px-4 py-3 text-slate-500 dark:text-slate-400">{formatDateTime(row.createdOn)}</td>
                      <td className="px-4 py-3 text-right">
                        <button
                          type="button"
                          onClick={() => router.push(`/dashboard/receipts?historyId=${row.receiptHistoryId}`)}
                          className="inline-flex items-center gap-1.5 rounded-lg border border-slate-300 px-2.5 py-1 text-xs font-medium text-slate-600 transition hover:bg-slate-50 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800"
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

            <div className="flex justify-end gap-2 pt-1">
              <button
                type="button"
                onClick={() => router.push(`/dashboard/payments?memberId=${historyTarget.memberId}`)}
                className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-50 dark:border-slate-700 dark:text-slate-300 dark:hover:bg-slate-800"
              >
                Record Payment
              </button>
              <button
                type="button"
                onClick={closeHistory}
                className="rounded-lg px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-800"
              >
                Close
              </button>
            </div>
          </div>
        )}
      </Modal>
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
