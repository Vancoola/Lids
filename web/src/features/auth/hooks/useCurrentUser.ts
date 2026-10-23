import { useQuery } from "@tanstack/react-query";
import { getCurrentUser } from "../api/auth.api";

export const AUTH_QUERY_KEY = ["auth", "me"] as const;

export function useCurrentUser() {
    return useQuery({
        queryKey: AUTH_QUERY_KEY,
        queryFn: getCurrentUser,
        retry: false,
        staleTime: 30_000,
        refetchOnWindowFocus: true,
    });
}