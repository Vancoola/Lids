import {Bot, Layers3} from "lucide-react";
import {useLocation, useNavigate} from "react-router";

import {
    Sidebar,
    SidebarContent,
    SidebarGroup,
    SidebarGroupContent,
    SidebarGroupLabel,
    SidebarHeader,
    SidebarMenu,
    SidebarMenuButton,
    SidebarMenuItem,
} from "@/components/ui/sidebar";
import {NavUser} from "@/components/shared/NavUser.tsx";

export function AppSidebar() {
    const navigate = useNavigate();
    const {pathname} = useLocation();

    return (
        <Sidebar collapsible="icon" variant="inset">
            <SidebarHeader className="p-4">
                <div className="flex items-center gap-2">
                    <Layers3 className="size-5 shrink-0"/>
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
                                    <Bot/>
                                    <span>Агенты</span>
                                </SidebarMenuButton>
                            </SidebarMenuItem>
                        </SidebarMenu>
                    </SidebarGroupContent>
                </SidebarGroup>
            </SidebarContent>
            <NavUser/>
        </Sidebar>
    );
}