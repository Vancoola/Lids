import {Outlet} from "react-router";
import {ModeToggle} from "@/components/mode-toggle";

export function AuthLayout() {
    return (
        <div className="relative flex min-h-svh items-center justify-center overflow-hidden bg-background px-4 py-12">
            <div aria-hidden="true" className="auth-glow pointer-events-none absolute inset-0"/>
            <div className="absolute right-4 top-4 z-10">
                <ModeToggle/>
            </div>
            <main className="relative w-full max-w-[420px]">
                <Outlet/>
            </main>
            <footer className="absolute bottom-4 text-xs text-muted-foreground">
                © {new Date().getFullYear()} Lids
            </footer>
        </div>
    );
}
