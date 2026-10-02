import { useCallback, useEffect, useState } from "react";
import { ApiError } from "@/lib/api";

type ResourceApi<T, TInput> = {
  list: () => Promise<T[]>;
  create: (input: TInput) => Promise<T>;
  update: (id: number, input: TInput) => Promise<T>;
  remove: (id: number) => Promise<void>;
};

function messageFrom(error: unknown, fallback: string) {
  return error instanceof ApiError ? error.message : fallback;
}

export function useCrudResource<T, TInput>(api: ResourceApi<T, TInput>, getId: (item: T) => number) {
  const [items, setItems] = useState<T[] | null>(null);
  const [loadError, setLoadError] = useState<string | null>(null);

  const [modalMode, setModalMode] = useState<"create" | "edit" | null>(null);
  const [editingItem, setEditingItem] = useState<T | null>(null);
  const [saving, setSaving] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  const [deleteTarget, setDeleteTarget] = useState<T | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoadError(null);
    try {
      setItems(await api.list());
    } catch (error) {
      setItems([]);
      setLoadError(messageFrom(error, "Failed to load data."));
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  function openCreate() {
    setModalMode("create");
    setEditingItem(null);
    setFormError(null);
  }

  function openEdit(item: T) {
    setModalMode("edit");
    setEditingItem(item);
    setFormError(null);
  }

  function closeModal() {
    if (saving) return;
    setModalMode(null);
    setEditingItem(null);
    setFormError(null);
  }

  async function submit(input: TInput) {
    setSaving(true);
    setFormError(null);
    try {
      if (modalMode === "edit" && editingItem) {
        await api.update(getId(editingItem), input);
      } else {
        await api.create(input);
      }
      setModalMode(null);
      setEditingItem(null);
      await load();
      return true;
    } catch (error) {
      setFormError(messageFrom(error, "Something went wrong. Please try again."));
      return false;
    } finally {
      setSaving(false);
    }
  }

  function confirmDelete(item: T) {
    setDeleteError(null);
    setDeleteTarget(item);
  }

  function cancelDelete() {
    if (deleting) return;
    setDeleteTarget(null);
  }

  async function handleDelete() {
    if (!deleteTarget) return;
    setDeleting(true);
    setDeleteError(null);
    try {
      await api.remove(getId(deleteTarget));
      setDeleteTarget(null);
      await load();
    } catch (error) {
      setDeleteError(messageFrom(error, "Failed to delete."));
    } finally {
      setDeleting(false);
    }
  }

  return {
    items,
    loadError,
    reload: load,
    modalMode,
    editingItem,
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
  };
}
