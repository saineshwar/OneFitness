export type FieldErrors = Record<string, string>;

export function isBlank(value: string | number | null | undefined) {
  if (value === null || value === undefined) return true;
  if (typeof value === "number") return false;
  return value.trim().length === 0;
}

/** For required text fields. */
export function required(value: string | null | undefined, message = "This field is required."): string | undefined {
  return isBlank(value) ? message : undefined;
}

/** For required FK/select fields modeled as number, where 0/empty means "not selected". */
export function requiredId(value: number | null | undefined, message = "Please select an option."): string | undefined {
  return !value ? message : undefined;
}

/** Runs a regex only when the value is non-empty (pair with `required` for mandatory fields). */
export function matches(value: string, regex: RegExp, message: string): string | undefined {
  if (!value) return undefined;
  return regex.test(value) ? undefined : message;
}

export const patterns = {
  name: /^[a-zA-Z ]*$/,
  mobile: /^[7-9][0-9]{9}$/,
  email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
};

/** Drops undefined entries so callers can check `Object.keys(errors).length === 0`. */
export function collectErrors(entries: Record<string, string | undefined>): FieldErrors {
  const errors: FieldErrors = {};
  for (const [key, value] of Object.entries(entries)) {
    if (value) errors[key] = value;
  }
  return errors;
}
