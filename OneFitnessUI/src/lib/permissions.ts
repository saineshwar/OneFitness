export const ADMIN_ONLY_ROUTES = [
  "/dashboard/refunds",
  "/dashboard/settings",
  "/dashboard/membership-types",
  "/dashboard/installments",
  "/dashboard/workouts",
  "/dashboard/tax-master",
  "/dashboard/reports",
  "/dashboard/roles",
  "/dashboard/users",
];

export function isAdminOnlyRoute(pathname: string): boolean {
  return ADMIN_ONLY_ROUTES.some((route) => pathname === route || pathname.startsWith(`${route}/`));
}

export function canAccessRoute(role: string | null, pathname: string): boolean {
  if (role === "Admin") return true;
  return !isAdminOnlyRoute(pathname);
}
