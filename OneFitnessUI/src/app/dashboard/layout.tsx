"use client";

import { useEffect, useState } from "react";
import { useRouter, usePathname } from "next/navigation";
import { Sidebar } from "@/components/Sidebar";
import { Topbar } from "@/components/Topbar";
import { Icon } from "@/components/icons";
import { getSession, type AuthUser } from "@/lib/auth";
import { canAccessRoute } from "@/lib/permissions";

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
  const [user, setUser] = useState<AuthUser | null>(null);
  const [checking, setChecking] = useState(true);
  const [sidebarOpen, setSidebarOpen] = useState(true);

  useEffect(() => {
    const session = getSession();
    if (!session) {
      router.replace("/login");
      return;
    }
    setUser(session);
    setChecking(false);

    if (window.innerWidth < 1024) {
      setSidebarOpen(false);
    }
  }, [router]);

  function closeSidebarOnMobile() {
    if (window.innerWidth < 1024) {
      setSidebarOpen(false);
    }
  }

  if (checking || !user) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-50 dark:bg-slate-950">
        <p className="text-sm text-slate-500 dark:text-slate-400">Loading…</p>
      </div>
    );
  }

  const allowed = canAccessRoute(user.role, pathname);

  return (
    <div className="flex min-h-screen bg-slate-50 dark:bg-slate-950">
      <Sidebar open={sidebarOpen} role={user.role} onNavigate={closeSidebarOnMobile} onClose={() => setSidebarOpen(false)} />

      <div
        className={`flex min-h-screen flex-1 flex-col transition-[margin] duration-200 ease-in-out ${
          sidebarOpen ? "lg:ml-64" : "lg:ml-20"
        }`}
      >
        <Topbar user={user} onMenuClick={() => setSidebarOpen((value) => !value)} />
        <main className="flex-1 overflow-y-auto p-4 sm:p-6 lg:p-8">
          {allowed ? (
            children
          ) : (
            <div className="mx-auto flex max-w-lg flex-col items-center justify-center gap-3 rounded-xl border border-slate-200/70 bg-white p-10 text-center shadow-sm dark:border-slate-800 dark:bg-slate-900">
              <div className="flex h-12 w-12 items-center justify-center rounded-full bg-red-50 text-red-600 dark:bg-red-500/10 dark:text-red-300">
                <Icon name="lock" className="h-6 w-6" />
              </div>
              <h1 className="text-lg font-semibold text-slate-900 dark:text-white">Access denied</h1>
              <p className="text-sm text-slate-500 dark:text-slate-400">
                Your account doesn&apos;t have permission to view this page. Contact an administrator if you believe this is
                a mistake.
              </p>
            </div>
          )}
        </main>
      </div>
    </div>
  );
}
