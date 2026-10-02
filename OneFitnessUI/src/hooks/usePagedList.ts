import { useCallback, useEffect, useState } from "react";
import { ApiError } from "@/lib/api";
import type { PagedResult } from "@/lib/api";

type PagedListFetcher<T> = (params: { page: number; pageSize: number; search: string }) => Promise<PagedResult<T>>;

const SEARCH_DEBOUNCE_MS = 300;

export function usePagedList<T>(fetcher: PagedListFetcher<T>, pageSize = 10) {
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");

  const [items, setItems] = useState<T[] | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);

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
      const result = await fetcher({ page, pageSize, search });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (error) {
      setItems([]);
      setTotalCount(0);
      setLoadError(error instanceof ApiError ? error.message : "Failed to load data.");
    } finally {
      setLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, pageSize, search]);

  useEffect(() => {
    load();
  }, [load]);

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

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
  };
}
