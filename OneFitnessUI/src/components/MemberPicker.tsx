"use client";

import { useEffect, useRef, useState } from "react";
import { Icon } from "@/components/icons";
import { fieldInputClass, inputBorderClass } from "@/components/Field";
import type { Member } from "@/lib/member";

export function memberFullName(member: Member) {
  return [member.firstName, member.middleName, member.lastName].filter(Boolean).join(" ");
}

type MemberPickerProps = {
  members: Member[];
  selected: Member | null;
  onSelect: (member: Member) => void;
  onClear: () => void;
  error?: string;
  placeholder?: string;
};

export function MemberPicker({ members, selected, onSelect, onClear, error, placeholder }: MemberPickerProps) {
  const [query, setQuery] = useState("");
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  if (selected) {
    return (
      <div className="flex items-center justify-between rounded-lg border border-slate-300 bg-slate-50 px-3 py-2 text-sm dark:border-slate-700 dark:bg-slate-800">
        <div>
          <p className="font-medium text-slate-800 dark:text-slate-200">{memberFullName(selected)}</p>
          <p className="text-xs text-slate-500 dark:text-slate-400">
            {selected.memberNo} {selected.mobileNo ? `· ${selected.mobileNo}` : ""}
          </p>
        </div>
        <button
          type="button"
          onClick={() => {
            onClear();
            setQuery("");
          }}
          className="text-xs font-medium text-indigo-600 hover:text-indigo-500 dark:text-indigo-400"
        >
          Change
        </button>
      </div>
    );
  }

  const q = query.trim().toLowerCase();
  const results = (
    q
      ? members.filter(
          (m) =>
            memberFullName(m).toLowerCase().includes(q) ||
            (m.mobileNo?.includes(q) ?? false) ||
            m.memberNo.toLowerCase().includes(q),
        )
      : members
  ).slice(0, 20);

  return (
    <div className="relative" ref={containerRef}>
      <div className="relative">
        <Icon name="search" className="pointer-events-none absolute top-1/2 left-3 h-4 w-4 -translate-y-1/2 text-slate-400" />
        <input
          value={query}
          onChange={(event) => {
            setQuery(event.target.value);
            setOpen(true);
          }}
          onFocus={() => setOpen(true)}
          placeholder={placeholder ?? "Search by name, mobile or member no."}
          className={`${fieldInputClass} pl-9 ${inputBorderClass(!!error)}`}
        />
      </div>

      {open && (
        <div className="absolute z-10 mt-1 max-h-64 w-full overflow-y-auto rounded-lg border border-slate-200 bg-white py-1 shadow-lg dark:border-slate-700 dark:bg-slate-900">
          {results.length === 0 && <p className="px-3 py-2 text-sm text-slate-400">No members found.</p>}
          {results.map((member) => (
            <button
              key={member.memberId}
              type="button"
              onClick={() => {
                onSelect(member);
                setOpen(false);
              }}
              className="flex w-full flex-col items-start px-3 py-2 text-left text-sm transition hover:bg-slate-50 dark:hover:bg-slate-800"
            >
              <span className="font-medium text-slate-800 dark:text-slate-200">{memberFullName(member)}</span>
              <span className="text-xs text-slate-500 dark:text-slate-400">
                {member.memberNo} {member.mobileNo ? `· ${member.mobileNo}` : ""}
              </span>
            </button>
          ))}
        </div>
      )}

      {error && <p className="mt-1 text-xs text-red-600 dark:text-red-400">{error}</p>}
    </div>
  );
}
