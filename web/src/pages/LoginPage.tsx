import {useState} from "react";
import {Eye, EyeOff, Layers3, LockKeyhole, Mail} from "lucide-react";

import {Button} from "@/components/ui/button";
import {Input} from "@/components/ui/input";
import {useNavigate} from "react-router";
import {useQueryClient} from "@tanstack/react-query";
import {getCurrentUser, login} from "@/features/auth/api/auth.api.ts";
import {AUTH_QUERY_KEY} from "@/features/auth/hooks/useCurrentUser.ts";

export function LoginPage() {
    const [showPassword, setShowPassword] = useState(false);

    const navigate = useNavigate();
    const queryClient = useQueryClient();

    const [isSubmitting, setIsSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);


    return (
        <div className="space-y-8">
            {/* Branding */}
            <div className="flex items-center justify-center gap-2.5">
                <div className="flex size-10 items-center justify-center rounded-xl bg-primary text-primary-foreground">
                    <Layers3 className="size-5"/>
                </div>

                <span className="text-2xl font-bold tracking-tight">
          Lids
        </span>
            </div>

            {/* Login card */}
            <section className="rounded-2xl border bg-card p-8 text-card-foreground shadow-panel sm:p-9">
                <div className="mb-8 space-y-2 text-center">
                    <h1 className="text-2xl font-semibold tracking-tight">
                        Добро пожаловать
                    </h1>

                    <p className="text-sm text-muted-foreground">
                        Войдите в аккаунт, чтобы продолжить работу
                    </p>
                </div>

                <form
                    className="space-y-5"
                    onSubmit={async (event) => {
                        event.preventDefault();

                        if (isSubmitting) return;

                        const formData = new FormData(event.currentTarget);

                        const email = String(formData.get("email") ?? "");
                        const password = String(formData.get("password") ?? "");

                        setIsSubmitting(true);
                        setError(null);

                        try {
                            await login(email, password);

                            const user = await getCurrentUser();

                            if (!user) {
                                throw new Error("Сессия не была создана");
                            }

                            queryClient.setQueryData(AUTH_QUERY_KEY, user);

                            navigate("/agents", {replace: true});
                        } catch (err) {
                            setError(
                                err instanceof Error
                                    ? err.message
                                    : "Не удалось выполнить вход"
                            );
                        } finally {
                            setIsSubmitting(false);
                        }
                    }}
                >
                    {/* Email */}
                    <div className="space-y-2">
                        <label
                            htmlFor="email"
                            className="text-sm font-medium"
                        >
                            Email
                        </label>

                        <div className="relative">
                            <Mail
                                className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground"/>

                            <Input
                                id="email"
                                name="email"
                                type="email"
                                placeholder="you@example.com"
                                autoComplete="email"
                                required
                                className="h-11 pl-10"
                            />
                        </div>
                    </div>

                    {/* Password */}
                    <div className="space-y-2">
                        <label
                            htmlFor="password"
                            className="text-sm font-medium"
                        >
                            Пароль
                        </label>

                        <div className="relative">
                            <LockKeyhole
                                className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground"/>

                            <Input
                                id="password"
                                name="password"
                                type={showPassword ? "text" : "password"}
                                placeholder="Введите пароль"
                                autoComplete="current-password"
                                required
                                className="h-11 pr-11 pl-10"
                            />

                            <button
                                type="button"
                                onClick={() => setShowPassword(!showPassword)}
                                aria-label={
                                    showPassword
                                        ? "Скрыть пароль"
                                        : "Показать пароль"
                                }
                                className="absolute top-1/2 right-3 -translate-y-1/2 text-muted-foreground transition-colors hover:text-foreground"
                            >
                                {showPassword ? (
                                    <EyeOff className="size-4"/>
                                ) : (
                                    <Eye className="size-4"/>
                                )}
                            </button>
                        </div>
                    </div>

                    {/* Submit */}
                    <Button
                        type="submit"
                        disabled={isSubmitting}
                        className="h-11 w-full"
                    >
                        {isSubmitting ? "Выполняется вход..." : "Войти"}
                    </Button>

                    {error && (
                        <p role="alert" className="text-sm text-destructive">
                            {error}
                        </p>
                    )}
                </form>
            </section>

            <p className="text-center text-xs text-muted-foreground">
                Доступ к системе предоставляется администратором
            </p>
        </div>
    );
}
