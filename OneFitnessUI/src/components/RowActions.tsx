import { Icon } from "@/components/icons";

type RowActionsProps = {
  label: string;
  onEdit: () => void;
  onDelete: () => void;
};

export function RowActions({ label, onEdit, onDelete }: RowActionsProps) {
  return (
    <div className="flex items-center justify-end gap-1">
      <button
        type="button"
        onClick={onEdit}
        aria-label={`Edit ${label}`}
        className="rounded-lg p-2 text-slate-400 transition hover:bg-slate-100 hover:text-indigo-600 dark:hover:bg-slate-800 dark:hover:text-indigo-300"
      >
        <Icon name="edit" className="h-4 w-4" />
      </button>
      <button
        type="button"
        onClick={onDelete}
        aria-label={`Delete ${label}`}
        className="rounded-lg p-2 text-slate-400 transition hover:bg-red-50 hover:text-red-600 dark:hover:bg-red-950 dark:hover:text-red-300"
      >
        <Icon name="trash" className="h-4 w-4" />
      </button>
    </div>
  );
}
