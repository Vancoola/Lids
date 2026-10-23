import { type ReactNode, useEffect, useState } from "react";
import { ThemeContext, type Theme } from "./theme-context";

type ThemeProviderProps = {
    children: ReactNode;
    defaultTheme?: Theme;
    storageKey?: string;
};

export function ThemeProvider({
                                  children,
                                  defaultTheme = "light",
                                  storageKey = "lids-theme",
                              }: ThemeProviderProps) {
    const [theme, setThemeState] = useState<Theme>(() => {
        const stored = localStorage.getItem(storageKey);
        return stored === "light" || stored === "dark" || stored === "system"
            ? stored
            : defaultTheme;
    });

    useEffect(() => {
        const media = window.matchMedia("(prefers-color-scheme: dark)");
        const root = document.documentElement;

        const applyTheme = () => {
            const dark = theme === "dark" || (theme === "system" && media.matches);
            root.classList.toggle("dark", dark);
            root.style.colorScheme = dark ? "dark" : "light";
        };

        applyTheme();
        media.addEventListener("change", applyTheme);
        return () => media.removeEventListener("change", applyTheme);
    }, [theme]);

    const setTheme = (value: Theme) => {
        localStorage.setItem(storageKey, value);
        setThemeState(value);
    };

    return (
        <ThemeContext.Provider value={{theme, setTheme}}>
            {children}
        </ThemeContext.Provider>
    );
}
