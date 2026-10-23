import { Bot, Layers3 } from "lucide-react";
import { useLocation, useNavigate } from "react-router";

import {
    Sidebar,
    SidebarHeader,
    SidebarContent,
    SidebarGroup,
    SidebarGroupLabel,
    SidebarGroupContent,
    SidebarMenu,
    SidebarMenuItem,
    SidebarMenuButton,
} from "@/components/ui/sidebar";

export function AppSidebar() {
    const navigate = useNavigate();
    const { pathname } = useLocation();

    return (
        <Sidebar collapsible="icon">
            <SidebarHeader className="p-4">
                <div className="flex items-center gap-2">
                    <Layers3 className="size-5 shrink-0" />
                    <span className="font-semibold group-data-[collapsible=icon]:hidden">
            Lids
          </span>
                </div>
            </SidebarHeader>

            <SidebarContent>
                <SidebarGroup>
                    <SidebarGroupLabel>Управление</SidebarGroupLabel>

                    <SidebarGroupContent>
                        <SidebarMenu>
                            <SidebarMenuItem>
                                <SidebarMenuButton
                                    tooltip="Агенты"
                                    isActive={pathname.startsWith("/agents")}
                                    onClick={() => navigate("/agents")}
                                >
                                    <Bot />
                                    <span>Агенты</span>
                                </SidebarMenuButton>
                            </SidebarMenuItem>
                        </SidebarMenu>
                    </SidebarGroupContent>
                </SidebarGroup>
            </SidebarContent>
        </Sidebar>
    );
}