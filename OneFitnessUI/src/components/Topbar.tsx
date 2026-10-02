"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter, usePathname } from "next/navigation";
import { Icon } from "@/components/icons";
import { logout, type AuthUser } from "@/lib/auth";

const pageTitles: Record<string, string> = {
  "/dashboard": "Dashboard",
  "/dashboard/members": "Members",
  "/dashboard/enquiries": "Enquiries",
  "/dashboard/reasons": "Enquiry Reasons",
  "/dashboard/renewal": "Renewal",
  "/dashboard/payments": "Payments",
  "/dashboard/refunds": "Refunds",
  "/dashboard/receipts": "Generate Receipt",
  "/dashboard/membership-types": "Membership Types",
  "/dashboard/installments": "Installments",
  "/dashboard/workouts": "Workouts",
  "/dashboard/tax-master": "Tax Master",
  "/dashboard/reports": "Reports",
  "/dashboard/roles": "Role Master",
  "/dashboard/users": "Users",
  "/dashboard/settings": "General Settings",
};

type TopbarProps = {
  user: AuthUser;
  onMenuClick: () => void;
};

export function Topbar({ user, onMenuClick }: TopbarProps) {
  const router = useRouter();
  const pathname = usePathname();
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
        setMenuOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function handleLogout() {
    logout();
    router.push("/login");
  }

  return (
    <header className="flex h-16 shrink-0 items-center justify-between border-b border-slate-200 bg-white/80 px-4 backdrop-blur-sm sm:px-6 dark:border-slate-800 dark:bg-slate-900/80">
      <div className="flex items-center gap-3">
        <button
          type="button"
          onClick={onMenuClick}
          aria-label="Toggle sidebar"
          className="rounded-lg p-2 text-slate-500 transition hover:bg-slate-100 hover:text-slate-700 dark:text-slate-400 dark:hover:bg-slate-800 dark:hover:text-slate-200"
        >
          <Icon name="menu" />
        </button>
        <span className="hidden h-6 w-px bg-slate-200 sm:block dark:bg-slate-800" />
        <h1 className="hidden text-base font-semibold tracking-tight text-slate-900 sm:block dark:text-white">
          {pageTitles[pathname] ?? "Dashboard"}
        </h1>
      </div>

      <div className="relative" ref={menuRef}>
        <button
          type="button"
          onClick={() => setMenuOpen((value) => !value)}
          aria-haspopup="menu"
          aria-expanded={menuOpen}
          className="flex items-center gap-2.5 rounded-lg py-1.5 pr-2 pl-2.5 transition hover:bg-slate-100 dark:hover:bg-slate-800"
        >
          <div className="flex h-9 w-9 items-center justify-center rounded-full bg-indigo-100 text-sm font-semibold text-indigo-700 dark:bg-indigo-500/15 dark:text-indigo-300">
            {user.name.charAt(0).toUpperCase()}
          </div>
          <div className="hidden text-left sm:block">
            <p className="text-sm leading-tight font-medium text-slate-800 dark:text-slate-200">{user.name}</p>
            <p className="text-xs leading-tight text-slate-400">{user.role}</p>
          </div>
          <Icon
            name="chevronDown"
            className={`hidden h-4 w-4 text-slate-400 transition-transform sm:block ${menuOpen ? "rotate-180" : ""}`}
          />
        </button>

        {menuOpen && (
          <div
            role="menu"
            className="absolute right-0 top-full mt-2 w-52 overflow-hidden rounded-xl border border-slate-200 bg-white py-1.5 shadow-lg shadow-slate-900/5 dark:border-slate-700 dark:bg-slate-900"
          >
            <div className="border-b border-slate-100 px-4 py-2.5 dark:border-slate-800">
              <p className="text-sm font-medium text-slate-800 dark:text-slate-200">{user.name}</p>
              <p className="text-xs text-slate-400">{user.role}</p>
            </div>
            <button
              type="button"
              role="menuitem"
              onClick={handleLogout}
              className="flex w-full items-center gap-2.5 px-4 py-2.5 text-sm text-slate-600 transition hover:bg-slate-50 dark:text-slate-300 dark:hover:bg-slate-800"
            >
              <Icon name="logout" className="h-4 w-4" />
              Logout
            </button>
          </div>
        )}
      </div>
    </header>
  );
}
