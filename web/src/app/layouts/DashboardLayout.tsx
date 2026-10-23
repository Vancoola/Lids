import { Outlet } from "react-router";

import {
    SidebarProvider,
    SidebarInset,
    SidebarTrigger,
} from "@/components/ui/sidebar";

import { Separator } from "@/components/ui/separator";
import { AppSidebar } from "@/components/shared/AppSidebar";

export function DashboardLayout() {
    return (
        <SidebarProvider>
            <AppSidebar />

            <SidebarInset>
                <header className="flex h-14 shrink-0 items-center gap-3 border-b px-4">
                    <SidebarTrigger />

                    <Separator
                        orientation="vertical"
                        className="h-4"
                    />

                    <span className="text-sm font-medium">
            Lids
          </span>
                </header>

                <main className="flex-1 p-6">
                    <Outlet />
                </main>
            </SidebarInset>
        </SidebarProvider>
    );
}