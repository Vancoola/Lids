import {Outlet} from "react-router";
import {SidebarInset, SidebarProvider, SidebarTrigger,} from "@/components/ui/sidebar";
import {Separator} from "@/components/ui/separator";
import {AppSidebar} from "@/components/shared/AppSidebar";
import {ModeToggle} from "@/components/mode-toggle";

export function DashboardLayout() {
    return (
        <SidebarProvider className="bg-background">
            <AppSidebar />
            <SidebarInset className="min-w-0 overflow-hidden rounded-2xl border border-border bg-card shadow-panel">
                <header className="flex h-14 shrink-0 items-center gap-3 border-b border-border/70 px-4">
                    <SidebarTrigger />
                    <Separator orientation="vertical" className="h-4"/>
                    <span className="text-sm font-semibold tracking-tight">Lids</span>
                    <div className="ml-auto">
                        <ModeToggle/>
                    </div>
                </header>
                <main className="flex-1 p-4 md:p-6">
                    <Outlet />
                </main>
            </SidebarInset>
        </SidebarProvider>
    );
}
