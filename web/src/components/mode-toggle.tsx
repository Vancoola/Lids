import {Check, Monitor, Moon, Sun} from "lucide-react";
import {Button} from "@/components/ui/button";
import {DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger,} from "@/components/ui/dropdown-menu";
import {useTheme} from "@/components/use-theme";

export function ModeToggle() {
    const {theme, setTheme} = useTheme();
    const Icon = theme === "dark" ? Moon : theme === "system" ? Monitor : Sun;

    return (
        <DropdownMenu>
            <DropdownMenuTrigger
                render={<Button variant="outline" size="icon" aria-label="Сменить тему"/>}
            >
                <Icon className="size-4"/>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-44">
                <DropdownMenuItem onClick={() => setTheme("light")}>
                    <Sun className="size-4"/> Светлая
                    {theme === "light" && <Check className="ml-auto size-4"/>}
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => setTheme("dark")}>
                    <Moon className="size-4"/> Тёмная
                    {theme === "dark" && <Check className="ml-auto size-4"/>}
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => setTheme("system")}>
                    <Monitor className="size-4"/> Системная
                    {theme === "system" && <Check className="ml-auto size-4"/>}
                </DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}
