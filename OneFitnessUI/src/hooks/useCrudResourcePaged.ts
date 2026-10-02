import { useCallback, useEffect, useState } from "react";
import { ApiError } from "@/lib/api";
import type { PagedResult } from "@/lib/api";

type PagedResourceApi<T, TInput> = {
  list: (params: { page: number; pageSize: number; search: string }) => Promise<PagedResult<T>>;
  create: (input: TInput) => Promise<T>;
  update: (id: number, input: TInput) => Promise<T>;
  remove: (id: number) => Promise<void>;
};

function messageFrom(error: unknown, fallback: string) {
  return error instanceof ApiError ? error.message : fallback;
}

const SEARCH_DEBOUNCE_MS = 300;

export function useCrudResourcePaged<T, TInput>(
  api: PagedResourceApi<T, TInput>,
  getId: (item: T) => number,
  pageSize = 10,
) {
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");

  const [items, setItems] = useState<T[] | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);

  const [modalMode, setModalMode] = useState<"create" | "edit" | null>(null);
  const [editingItem, setEditingItem] = useState<T | null>(null);
  const [saving, setSaving] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  const [deleteTarget, setDeleteTarget] = useState<T | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);

  // Debounce the raw search input before it drives a request, and jump back to page 1.
  useEffect(() => {
    const timeout = setTimeout(() => {
      setSearch(searchInput);
      setPage(1);
    }, SEARCH_DEBOUNCE_MS);
    return () => clearTimeout(timeout);
  }, [searchInput]);

  const load = useCallback(async () => {
    setLoading(true);
    setLoadError(null);
    try {
      const result = await api.list({ page, pageSize, search });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (error) {
      setItems([]);
      setTotalCount(0);
      setLoadError(messageFrom(error, "Failed to load data."));
    } finally {
      setLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, pageSize, search]);

  useEffect(() => {
    load();
  }, [load]);

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

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

  async function submit(input: TInput): Promise<T | null> {
    setSaving(true);
    setFormError(null);
    try {
      const result =
        modalMode === "edit" && editingItem ? await api.update(getId(editingItem), input) : await api.create(input);
      setModalMode(null);
      setEditingItem(null);
      await load();
      return result;
    } catch (error) {
      setFormError(messageFrom(error, "Something went wrong. Please try again."));
      return null;
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
    loading,
    loadError,
    reload: load,

    page,
    setPage,
    totalPages,
    totalCount,
    pageSize,
    searchInput,
    setSearchInput,

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
