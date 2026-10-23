
const API_BASE_URL = "/api";

export async function apiFetch(
    path: string,
    options: RequestInit = {}
): Promise<Response> {
    const url = `${API_BASE_URL}/${path.replace(/^\/+/, "")}`;

    return fetch(url, {
        ...options,
        credentials: "include",
        headers: {
            ...(options.body instanceof FormData
                ? {}
                : options.body != null
                    ? { "Content-Type": "application/json" }
                    : {}),
            ...options.headers,
        },
    });
}
