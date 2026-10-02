"use client";

import { useEffect, useState, type FormEvent } from "react";
import { Modal } from "@/components/Modal";
import { ConfirmDeleteModal } from "@/components/ConfirmDeleteModal";
import { StatusBadge } from "@/components/StatusBadge";
import { ToggleSwitch } from "@/components/ToggleSwitch";
import { RowActions } from "@/components/RowActions";
import { Field, fieldInputClass, inputBorderClass } from "@/components/Field";
import { Icon } from "@/components/icons";
import { createUser, deleteUser, getUsers, resetUserPassword, updateUser, type User } from "@/lib/user";
import { getActiveRoles, type Role } from "@/lib/roleMaster";
import { ApiError } from "@/lib/api";
import { collectErrors, matches, patterns, required, requiredId, type FieldErrors } from "@/lib/validation";

const genders = [
  { id: "M", label: "Male" },
  { id: "F", label: "Female" },
];

function formatDate(value: string) {
  return new Date(value).toLocaleDateString("en-IN", { year: "numeric", month: "short", day: "numeric" });
}

const emptyForm = {
  userName: "",
  firstName: "",
  lastName: "",
  emailId: "",
  mobileNo: "",
  gender: "",
  roleId: 0,
  password: "",
  confirmPassword: "",
  status: true,
};

export default function UsersPage() {
  const [users, setUsers] = useState<User[] | null>(null);
  const [loadError, setLoadError] = useState<string | null>(null);

  async function loadUsers() {
    setLoadError(null);
    try {
      setUsers(await getUsers());
    } catch (error) {
      setUsers([]);
      setLoadError(error instanceof ApiError ? error.message : "Failed to load users.");
    }
  }

  useEffect(() => {
    loadUsers();
  }, []);

  const [roles, setRoles] = useState<Role[]>([]);

  useEffect(() => {
    getActiveRoles().then(setRoles).catch(() => setRoles([]));
  }, []);

  const [modalMode, setModalMode] = useState<"create" | "edit" | null>(null);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [fieldErrors, setFieldErrors] = useState<FieldErrors>({});
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const [deleteTarget, setDeleteTarget] = useState<User | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const [resetTarget, setResetTarget] = useState<User | null>(null);
  const [resetForm, setResetForm] = useState({ newPassword: "", confirmPassword: "" });
  const [resetFieldErrors, setResetFieldErrors] = useState<FieldErrors>({});
  const [resetError, setResetError] = useState<string | null>(null);
  const [resetting, setResetting] = useState(false);

  function updateField<K extends keyof typeof form>(key: K, value: (typeof form)[K]) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function fullName(user: User) {
    return [user.firstName, user.lastName].filter(Boolean).join(" ");
  }

  function openCreate() {
    setForm(emptyForm);
    setFieldErrors({});
    setFormError(null);
    setEditingUser(null);
    setModalMode("create");
  }

  function openEdit(user: User) {
    setForm({
      userName: user.userName,
      firstName: user.firstName,
      lastName: user.lastName ?? "",
      emailId: user.emailId,
      mobileNo: user.mobileNo ?? "",
      gender: user.gender ?? "",
      roleId: user.roleId ?? 0,
      password: "",
      confirmPassword: "",
      status: user.status,
    });
    setFieldErrors({});
    setFormError(null);
    setEditingUser(user);
    setModalMode("edit");
  }

  function closeModal() {
    if (saving) return;
    setModalMode(null);
    setEditingUser(null);
  }

  function validate(): FieldErrors {
    const isCreate = modalMode === "create";
    return collectErrors({
      userName: isCreate ? required(form.userName, "Username is required.") : undefined,
      firstName: required(form.firstName, "First name is required."),
      emailId: required(form.emailId, "Email is required.") ?? matches(form.emailId, patterns.email, "Enter a valid email address."),
      roleId: requiredId(form.roleId, "Please select a role."),
      password: isCreate
        ? required(form.password, "Password is required.") ??
          (form.password.length < 6 ? "Password must be at least 6 characters." : undefined)
        : undefined,
      confirmPassword: isCreate
        ? required(form.confirmPassword, "Please confirm the password.") ??
          (form.confirmPassword !== form.password ? "Passwords do not match." : undefined)
        : undefined,
    });
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const errors = validate();
    setFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;

    setSaving(true);
    setFormError(null);
    try {
      if (modalMode === "edit" && editingUser) {
        await updateUser(editingUser.userId, {
          firstName: form.firstName.trim(),
          lastName: form.lastName.trim() || null,
          emailId: form.emailId.trim(),
          mobileNo: form.mobileNo.trim() || null,
          gender: form.gender || null,
          roleId: form.roleId,
          status: form.status,
        });
      } else {
        await createUser({
          userName: form.userName.trim(),
          firstName: form.firstName.trim(),
          lastName: form.lastName.trim() || null,
          emailId: form.emailId.trim(),
          mobileNo: form.mobileNo.trim() || null,
          gender: form.gender || null,
          roleId: form.roleId,
          password: form.password,
          confirmPassword: form.confirmPassword,
        });
      }
      setModalMode(null);
      setEditingUser(null);
      await loadUsers();
    } catch (error) {
      setFormError(error instanceof ApiError ? error.message : "Failed to save user.");
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    setDeleteError(null);
    try {
      await deleteUser(deleteTarget.userId);
      setDeleteTarget(null);
      await loadUsers();
    } catch (error) {
      setDeleteError(error instanceof ApiError ? error.message : "Failed to delete user.");
    } finally {
      setDeleting(false);
    }
  }

  function openResetPassword(user: User) {
    setResetTarget(user);
    setResetForm({ newPassword: "", confirmPassword: "" });
    setResetFieldErrors({});
    setResetError(null);
  }

  function closeResetPassword() {
    if (resetting) return;
    setResetTarget(null);
  }

  async function handleResetPassword(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!resetTarget) return;

    const errors = collectErrors({
      newPassword:
        required(resetForm.newPassword, "New password is required.") ??
        (resetForm.newPassword.length < 6 ? "Password must be at least 6 characters." : undefined),
      confirmPassword:
        required(resetForm.confirmPassword, "Please confirm the password.") ??
        (resetForm.confirmPassword !== resetForm.newPassword ? "Passwords do not match." : undefined),
    });
    setResetFieldErrors(errors);
    if (Object.keys(errors).length > 0) return;

    setResetting(true);
    setResetError(null);
    try {
      await resetUserPassword(resetTarget.userId, resetForm);
      setResetTarget(null);
    } catch (error) {
      setResetError(error instanceof ApiError ? error.message : "Failed to reset password.");
    } finally {
      setResetting(false);
    }
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-slate-900 dark:text-white">Users</h1>
          <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">Manage admin portal login accounts.</p>
        </div>
        <button
          type="button"
          onClick={openCreate}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm shadow-indigo-600/20 transition hover:bg-indigo-500"
        >
          <span className="text-base leading-none">+</span>
          Add User
        </button>
      </div>

      {loadError && (
        <div className="flex items-center justify-between gap-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900 dark:bg-red-950 dark:text-red-300">
          <span>{loadError}</span>
          <button
            type="button"
            onClick={loadUsers}
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
                <th className="px-5 py-3 font-medium">Username</th>
                <th className="px-5 py-3 font-medium">Name</th>
                <th className="px-5 py-3 font-medium">Email</th>
                <th className="px-5 py-3 font-medium">Mobile</th>
                <th className="px-5 py-3 font-medium">Role</th>
                <th className="px-5 py-3 font-medium">Status</th>
                <th className="px-5 py-3 font-medium">Created</th>
                <th className="px-5 py-3 font-medium text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100 dark:divide-slate-800">
              {users === null &&
                [0, 1, 2].map((i) => (
                  <tr key={i}>
                    <td className="px-5 py-4" colSpan={8}>
                      <div className="h-4 w-full animate-pulse rounded bg-slate-100 dark:bg-slate-800" />
                    </td>
                  </tr>
                ))}

              {users !== null && users.length === 0 && !loadError && (
                <tr>
                  <td colSpan={8} className="px-5 py-10 text-center text-sm text-slate-400">
                    No users yet. Click <span className="font-medium text-slate-600 dark:text-slate-300">Add User</span> to
                    create one.
                  </td>
                </tr>
              )}

              {users?.map((user) => (
                <tr key={user.userId} className="transition hover:bg-slate-50 dark:hover:bg-slate-800/50">
                  <td className="px-5 py-3.5 font-mono text-xs text-slate-500 dark:text-slate-400">{user.userName}</td>
                  <td className="px-5 py-3.5 font-medium text-slate-800 dark:text-slate-200">{fullName(user)}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{user.emailId}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{user.mobileNo ?? "—"}</td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{user.roleName ?? "—"}</td>
                  <td className="px-5 py-3.5">
                    <StatusBadge active={user.status} />
                  </td>
                  <td className="px-5 py-3.5 text-slate-500 dark:text-slate-400">{formatDate(user.createdOn)}</td>
                  <td className="px-5 py-3.5">
                    <div className="flex items-center justify-end gap-1">
                      <button
                        type="button"
                        onClick={() => openResetPassword(user)}
                        aria-label={`Reset password for ${user.userName}`}
                        title="Reset password"
                        className="rounded-lg p-2 text-slate-400 transition hover:bg-slate-100 hover:text-indigo-600 dark:hover:bg-slate-800 dark:hover:text-indigo-300"
                      >
                        <Icon name="lock" className="h-4 w-4" />
                      </button>
                      <RowActions label={user.userName} onEdit={() => openEdit(user)} onDelete={() => setDeleteTarget(user)} />
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <Modal open={modalMode !== null} onClose={closeModal} title={modalMode === "edit" ? "Edit User" : "Add User"} size="lg">
        <form onSubmit={handleSubmit} noValidate className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <Field label="Username" error={fieldErrors.userName}>
              <input
                value={form.userName}
                onChange={(e) => updateField("userName", e.target.value)}
                disabled={modalMode === "edit"}
                autoFocus={modalMode === "create"}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.userName)} ${
                  modalMode === "edit" ? "bg-slate-50 dark:bg-slate-800" : ""
                }`}
                placeholder="jdoe"
              />
            </Field>
            <Field label="Email" error={fieldErrors.emailId}>
              <input
                type="email"
                value={form.emailId}
                onChange={(e) => updateField("emailId", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.emailId)}`}
                placeholder="jane@example.com"
              />
            </Field>
            <Field label="First Name" error={fieldErrors.firstName}>
              <input
                value={form.firstName}
                onChange={(e) => updateField("firstName", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.firstName)}`}
                placeholder="Jane"
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
            <Field label="Gender" optional error={fieldErrors.gender}>
              <select
                value={form.gender}
                onChange={(e) => updateField("gender", e.target.value)}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.gender)}`}
              >
                <option value="">Select</option>
                {genders.map((g) => (
                  <option key={g.id} value={g.id}>
                    {g.label}
                  </option>
                ))}
              </select>
            </Field>
            <Field label="Role" error={fieldErrors.roleId}>
              <select
                value={form.roleId || ""}
                onChange={(e) => updateField("roleId", Number(e.target.value))}
                className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.roleId)}`}
              >
                <option value="" disabled>
                  Select
                </option>
                {roles.map((r) => (
                  <option key={r.roleId} value={r.roleId}>
                    {r.roleName}
                  </option>
                ))}
              </select>
            </Field>
          </div>

          {modalMode === "create" && (
            <div className="grid grid-cols-2 gap-3">
              <Field label="Password" error={fieldErrors.password}>
                <input
                  type="password"
                  value={form.password}
                  onChange={(e) => updateField("password", e.target.value)}
                  className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.password)}`}
                />
              </Field>
              <Field label="Confirm Password" error={fieldErrors.confirmPassword}>
                <input
                  type="password"
                  value={form.confirmPassword}
                  onChange={(e) => updateField("confirmPassword", e.target.value)}
                  className={`${fieldInputClass} ${inputBorderClass(!!fieldErrors.confirmPassword)}`}
                />
              </Field>
            </div>
          )}

          {modalMode === "edit" && (
            <ToggleSwitch label="Active" checked={form.status} onChange={(value) => updateField("status", value)} />
          )}

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
        itemLabel={deleteTarget?.userName ?? ""}
        deleting={deleting}
        error={deleteError}
        onCancel={() => setDeleteTarget(null)}
        onConfirm={handleDelete}
        title="Delete User"
      />

      <Modal
        open={resetTarget !== null}
        onClose={closeResetPassword}
        title={resetTarget ? `Reset Password — ${resetTarget.userName}` : "Reset Password"}
      >
        <form onSubmit={handleResetPassword} noValidate className="space-y-4">
          <Field label="New Password" error={resetFieldErrors.newPassword}>
            <input
              type="password"
              autoFocus
              value={resetForm.newPassword}
              onChange={(e) => setResetForm((current) => ({ ...current, newPassword: e.target.value }))}
              className={`${fieldInputClass} ${inputBorderClass(!!resetFieldErrors.newPassword)}`}
            />
          </Field>
          <Field label="Confirm Password" error={resetFieldErrors.confirmPassword}>
            <input
              type="password"
              value={resetForm.confirmPassword}
              onChange={(e) => setResetForm((current) => ({ ...current, confirmPassword: e.target.value }))}
              className={`${fieldInputClass} ${inputBorderClass(!!resetFieldErrors.confirmPassword)}`}
            />
          </Field>

          {resetError && (
            <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-700 dark:bg-red-950 dark:text-red-300">
              {resetError}
            </p>
          )}

          <div className="flex justify-end gap-2 pt-2">
            <button
              type="button"
              onClick={closeResetPassword}
              className="rounded-lg px-4 py-2 text-sm font-medium text-slate-600 transition hover:bg-slate-100 dark:text-slate-300 dark:hover:bg-slate-800"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={resetting}
              className="flex items-center gap-2 rounded-lg bg-indigo-600 px-4 py-2 text-sm font-semibold text-white shadow-sm transition hover:bg-indigo-500 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {resetting && <span className="h-4 w-4 animate-spin rounded-full border-2 border-white/40 border-t-white" />}
              {resetting ? "Resetting…" : "Reset Password"}
            </button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
