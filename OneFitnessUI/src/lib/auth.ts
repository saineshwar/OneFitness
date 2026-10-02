import { api, ApiError, AUTH_STORAGE_KEY } from "@/lib/api";

export type AuthUser = {
  token: string;
  expiresAtUtc: string;
  userId: number;
  userName: string;
  name: string;
  role: string | null;
};

type LoginResult = { success: true; user: AuthUser } | { success: false; message: string };

type LoginApiResponse = {
  token: string;
  expiresAtUtc: string;
  userId: number;
  userName: string;
  firstName: string;
  lastName: string | null;
  roleId: number | null;
  roleName: string | null;
};

export async function login(username: string, password: string): Promise<LoginResult> {
  if (!username.trim() || !password) {
    return { success: false, message: "Enter both username and password." };
  }

  try {
    const result = await api.post<LoginApiResponse>("/Auth/login", {
      userName: username.trim(),
      password,
    });

    const user: AuthUser = {
      token: result.token,
      expiresAtUtc: result.expiresAtUtc,
      userId: result.userId,
      userName: result.userName,
      name: [result.firstName, result.lastName].filter(Boolean).join(" "),
      role: result.roleName,
    };

    window.localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(user));
    return { success: true, user };
  } catch (error) {
    const message = error instanceof ApiError ? error.message : "Could not sign in. Please try again.";
    return { success: false, message };
  }
}

export function logout(): void {
  window.localStorage.removeItem(AUTH_STORAGE_KEY);
}

export function getSession(): AuthUser | null {
  if (typeof window === "undefined") {
    return null;
  }

  const raw = window.localStorage.getItem(AUTH_STORAGE_KEY);
  if (!raw) {
    return null;
  }

  try {
    const user = JSON.parse(raw) as AuthUser;
    if (new Date(user.expiresAtUtc).getTime() <= Date.now()) {
      window.localStorage.removeItem(AUTH_STORAGE_KEY);
      return null;
    }
    return user;
  } catch {
    return null;
  }
}
