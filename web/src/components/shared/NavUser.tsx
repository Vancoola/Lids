import {useState} from "react";
import {useNavigate} from "react-router";
import {useQueryClient} from "@tanstack/react-query";
import {ChevronsUpDown, LogOut, ShieldCheck} from "lucide-react";

import {SidebarFooter, SidebarMenu, SidebarMenuButton, SidebarMenuItem,} from "@/components/ui/sidebar";

import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

import {logout} from "@/features/auth/api/auth.api";
import {AUTH_QUERY_KEY, useCurrentUser,} from "@/features/auth/hooks/useCurrentUser";

export function NavUser() {
    const {data: user} = useCurrentUser();
    const queryClient = useQueryClient();
    const navigate = useNavigate();

    const [isLoggingOut, setIsLoggingOut] = useState(false);
    const [error, setError] = useState<string | null>(null);

    if (!user) return null;

    const initials = user.email.charAt(0).toUpperCase();

    const role = user.roles.includes("SuperAdmin")
        ? "Суперадминистратор"
        : user.roles.includes("Admin")
            ? "Администратор"
            : user.roles.includes("Operator")
                ? "Оператор"
                : "Пользователь";

    async function handleLogout() {
        if (isLoggingOut) return;

        setIsLoggingOut(true);
        setError(null);

        try {
            await logout();

            queryClient.clear();

            queryClient.setQueryData(AUTH_QUERY_KEY, null);

            navigate("/login", {replace: true});
        } catch (err) {
            setError(
                err instanceof Error
                    ? err.message
                    : "Ошибка выхода"
            );
        } finally {
            setIsLoggingOut(false);
        }
    }

    return (
        <SidebarFooter className="border-t border-sidebar-border/60 p-2">
            <SidebarMenu>
                <SidebarMenuItem>
                    <DropdownMenu>
                        <DropdownMenuTrigger
                            render={
                                <SidebarMenuButton
                                    size="lg"
                                    tooltip={user.email}
                                    disabled={isLoggingOut}
                                    className="group-data-[collapsible=icon]:size-8! group-data-[collapsible=icon]:p-0!"
                                />
                            }
                        >
                            <div
                                className="flex size-8 shrink-0 items-center justify-center rounded-lg bg-primary text-sm font-semibold text-primary-foreground">
                                {initials}
                            </div>

                            <div className="grid min-w-0 flex-1 gap-0.5 text-left text-sm leading-tight">
                                <span className="truncate font-medium">
                                  {user.email}
                                </span>
                                <span className="truncate text-xs text-muted-foreground">
                                  {role}
                                </span>
                            </div>

                            <ChevronsUpDown className="ml-auto size-4 shrink-0"/>
                        </DropdownMenuTrigger>


                        <DropdownMenuContent
                            side="top"
                            align="start"
                            sideOffset={10}
                            className="w-72 rounded-2xl border-border/70 bg-popover p-2 shadow-xl"
                        >
                            {/* Account header */}
                            <div className="flex items-center gap-3 rounded-xl bg-primary/5 p-3">
                                <div
                                    className="flex size-11 shrink-0 items-center justify-center rounded-xl bg-primary text-base font-semibold text-primary-foreground">
                                    {initials}
                                </div>

                                <div className="grid min-w-0 gap-0.5">
                                  <span className="truncate text-sm font-semibold">
                                    Администратор
                                  </span>

                                    <span className="truncate text-xs text-muted-foreground">
                                    {user.email}
                                  </span>
                                </div>
                            </div>

                            {/* User role */}
                            <div className="flex items-center gap-2 px-3 py-3 text-xs text-muted-foreground">
                                <ShieldCheck className="size-4 text-primary"/>
                                <span>{role}</span>
                            </div>

                            <DropdownMenuSeparator className="my-1"/>

                            {/* Logout */}
                            <DropdownMenuItem
                                disabled={isLoggingOut}
                                onClick={() => void handleLogout()}
                                className="h-11 cursor-pointer gap-3 rounded-lg px-3 text-destructive data-highlighted:bg-destructive/10 data-highlighted:text-destructive"
                            >
                                <LogOut className="size-4"/>
                                <span className="font-medium">
                                  {isLoggingOut ? "Выход..." : "Выйти из аккаунта"}
                                </span>
                            </DropdownMenuItem>
                        </DropdownMenuContent>

                    </DropdownMenu>
                </SidebarMenuItem>
            </SidebarMenu>

            {error && (
                <p role="alert" className="px-2 text-xs text-destructive">
                    {error}
                </p>
            )}
        </SidebarFooter>
    );
}
