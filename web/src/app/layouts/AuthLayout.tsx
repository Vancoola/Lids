import { Outlet } from "react-router";

export function AuthLayout() {
    return (
        <div className="relative flex min-h-svh items-center justify-center overflow-hidden bg-muted/30 px-4 py-12">
            <div className="pointer-events-none absolute inset-0 bg-[radial-gradient(ellipse_at_50%_20%,rgba(99,102,241,0.07),transparent_60%)]" />

            <main className="relative w-full max-w-[420px]">
                <Outlet />
            </main>

            <footer className="absolute bottom-5 text-xs text-muted-foreground">
                © {new Date().getFullYear()} Lids
            </footer>
        </div>
    );
}
