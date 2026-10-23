import { apiFetch } from "@/lib/api-client";
import type { CurrentUser } from "../types/auth";

export async function getCurrentUser():
    Promise<CurrentUser | null> {

    const response = await apiFetch("/auth/me");

    if (response.status === 401) {
        return null;
    }

    if (!response.ok) {
        throw new Error(
            `Failed to check session: ${response.status}`
        );
    }

    return response.json();
}

export async function login(email: string, password: string) {
    const csrfResponse = await apiFetch("/auth/csrf");

    if (!csrfResponse.ok) {
        throw new Error("Не удалось получить CSRF-токен");
    }

    const { token } = (await csrfResponse.json()) as {
        token: string;
    };

    const response = await apiFetch("/auth/login", {
        method: "POST",
        headers: {
            "X-CSRF-TOKEN": token,
        },
        body: JSON.stringify({ email, password }),
    });

    if (response.status === 401) {
        throw new Error("Неверный email или пароль");
    }

    if (!response.ok) {
        throw new Error(`Ошибка авторизации: ${response.status}`);
    }
}