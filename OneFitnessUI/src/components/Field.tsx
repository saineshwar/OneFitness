type FieldProps = {
  label: string;
  optional?: boolean;
  error?: string;
  className?: string;
  children: React.ReactNode;
};

export function Field({ label, optional, error, className, children }: FieldProps) {
  return (
    <div className={className}>
      <label className="block text-sm font-medium text-slate-700 dark:text-slate-300">
        <span className="mb-1.5 block">
          {label} {optional && <span className="font-normal text-slate-400">(optional)</span>}
        </span>
        {children}
      </label>
      {error && <p className="mt-1 text-xs text-red-600 dark:text-red-400">{error}</p>}
    </div>
  );
}

export const fieldInputClass =
  "w-full rounded-lg border bg-white px-3 py-2 text-sm text-slate-900 outline-none transition focus:ring-4 dark:bg-slate-900 dark:text-white";

export function inputBorderClass(hasError: boolean) {
  return hasError
    ? "border-red-400 focus:border-red-500 focus:ring-red-500/10 dark:border-red-500"
    : "border-slate-300 focus:border-indigo-500 focus:ring-indigo-500/10 dark:border-slate-700";
}
