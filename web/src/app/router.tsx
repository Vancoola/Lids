import {createBrowserRouter, Navigate} from "react-router";

import {AuthLayout} from "./layouts/AuthLayout";
import {DashboardLayout} from "./layouts/DashboardLayout";

import {LoginPage} from "@/pages/LoginPage";
import {AgentsPage} from "@/pages/AgentsPage";

import {ProtectedRoute} from "@/features/auth/components/ProtectedRoute";

export const router = createBrowserRouter([
    {
        path: "/login",
        Component: AuthLayout,
        children: [
            {index: true, Component: LoginPage},
        ],
    },
    {
        Component: ProtectedRoute,
        children: [
            {
                path: "/",
                Component: DashboardLayout,
                children: [
                    {
                        index: true,
                        element: <Navigate to="/agents" replace/>,
                    },
                    {
                        path: "agents",
                        Component: AgentsPage,
                    },
                ],
            },
        ],
    },
]);