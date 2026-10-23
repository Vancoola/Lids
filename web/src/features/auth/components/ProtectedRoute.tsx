import {Navigate, Outlet, useLocation} from "react-router";
import {LoaderCircle} from "lucide-react";

import {Button} from "@/components/ui/button";
import {useCurrentUser} from "../hooks/useCurrentUser";

export function ProtectedRoute() {
    const location = useLocation();

    const {
        data: user,
        isPending,
        isError,
        isFetching,
        refetch,
    } = useCurrentUser();

    if (isPending) {
        return (
            <div className="flex min-h-svh items-center justify-center">
                <LoaderCircle className="size-6 animate-spin text-muted-foreground"/>
            </div>
        );
    }

    if (isError) {
        return (
            <div className="flex min-h-svh flex-col items-center justify-center gap-4">
                <p className="text-sm text-muted-foreground">
                    Не удалось проверить авторизацию
                </p>

                <Button
                    variant="outline"
                    disabled={isFetching}
                    onClick={() => void refetch()}
                >
                    Повторить
                </Button>
            </div>
        );
    }

    if (!user) {
        return (
            <Navigate
                to="/login"
                replace
                state={{
                    from: location.pathname + location.search,
                }}
            />
        );
    }

    return <Outlet/>;
}
