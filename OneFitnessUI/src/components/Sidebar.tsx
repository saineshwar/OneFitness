"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Icon } from "@/components/icons";
import { isAdminOnlyRoute } from "@/lib/permissions";

type MenuItem = {
  label: string;
  href: string;
  icon: Parameters<typeof Icon>[0]["name"];
  comingSoon?: boolean;
};

const menuItems: MenuItem[] = [
  { label: "Dashboard", href: "/dashboard", icon: "dashboard" },
  { label: "Members", href: "/dashboard/members", icon: "members" },
  { label: "Enquiries", href: "/dashboard/enquiries", icon: "inbox" },
  { label: "Enquiry Reasons", href: "/dashboard/reasons", icon: "list" },
  { label: "Payments", href: "/dashboard/payments", icon: "creditCard" },
  { label: "Refunds", href: "/dashboard/refunds", icon: "refund" },
  { label: "Generate Receipt", href: "/dashboard/receipts", icon: "receipt" },
  { label: "Membership Types", href: "/dashboard/membership-types", icon: "tag" },
  { label: "Installments", href: "/dashboard/installments", icon: "creditCard" },
  { label: "Workouts", href: "/dashboard/workouts", icon: "activity" },
  { label: "Tax Master", href: "/dashboard/tax-master", icon: "percent" },
  { label: "Reports", href: "/dashboard/reports", icon: "chart" },
  { label: "Role Master", href: "/dashboard/roles", icon: "shield" },
  { label: "Users", href: "/dashboard/users", icon: "user" },
  { label: "Settings", href: "/dashboard/settings", icon: "cog" },
];

type SidebarProps = {
  open: boolean;
  role: string | null;
  onNavigate?: () => void;
  onClose?: () => void;
};

export function Sidebar({ open, role, onNavigate, onClose }: SidebarProps) {
  const pathname = usePathname();
  const visibleMenuItems = role === "Admin" ? menuItems : menuItems.filter((item) => !isAdminOnlyRoute(item.href));

  return (
    <>
      {open && (
        <button
          type="button"
          aria-label="Close sidebar"
          onClick={onClose}
          className="fixed inset-0 z-20 bg-slate-950/50 backdrop-blur-[1px] lg:hidden"
        />
      )}

      <aside
        className={`fixed inset-y-0 left-0 z-30 flex h-screen shrink-0 flex-col overflow-hidden bg-slate-900 text-slate-200 transition-all duration-200 ease-in-out ${
          open ? "w-64 translate-x-0" : "w-64 -translate-x-full lg:w-20 lg:translate-x-0"
        }`}
      >
        <div className={`flex h-16 items-center gap-2.5 border-b border-white/5 ${open ? "px-5" : "lg:justify-center lg:px-0"}`}>
          <div className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-indigo-600 text-sm font-bold text-white">
            OF
          </div>
          <span className={`text-base font-semibold tracking-tight whitespace-nowrap text-white ${open ? "" : "lg:hidden"}`}>
            OneFitness
          </span>
        </div>

        <nav className="flex-1 space-y-0.5 overflow-y-auto px-3 py-4">
          <p className={`px-3 pb-2 text-[11px] font-semibold tracking-wider text-slate-500 uppercase ${open ? "" : "lg:hidden"}`}>
            Menu
          </p>

          {visibleMenuItems.map((item) => {
            const active = pathname === item.href;
            const collapsedClasses = open ? "" : "lg:justify-center lg:px-0";

            if (item.comingSoon) {
              return (
                <div
                  key={item.href}
                  title={open ? "Coming soon" : item.label}
                  className={`flex cursor-not-allowed items-center justify-between rounded-lg px-3 py-2.5 text-sm text-slate-500 ${collapsedClasses}`}
                >
                  <span className="flex items-center gap-3">
                    <Icon name={item.icon} className="h-5 w-5 shrink-0" />
                    <span className={`whitespace-nowrap ${open ? "" : "lg:hidden"}`}>{item.label}</span>
                  </span>
                  <span
                    className={`rounded-full bg-slate-800 px-2 py-0.5 text-[10px] font-medium tracking-wide text-slate-400 uppercase ${
                      open ? "" : "lg:hidden"
                    }`}
                  >
                    Soon
                  </span>
                </div>
              );
            }

            return (
              <Link
                key={item.href}
                href={item.href}
                onClick={onNavigate}
                title={item.label}
                className={`flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition ${collapsedClasses} ${
                  active
                    ? "bg-indigo-600 text-white shadow-sm shadow-indigo-950/40"
                    : "text-slate-300 hover:bg-white/5 hover:text-white"
                }`}
              >
                <Icon name={item.icon} className="h-5 w-5 shrink-0" />
                <span className={`whitespace-nowrap ${open ? "" : "lg:hidden"}`}>{item.label}</span>
              </Link>
            );
          })}
        </nav>
      </aside>
    </>
  );
}
