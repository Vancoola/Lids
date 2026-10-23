import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import './index.css'

import {QueryClient, QueryClientProvider} from "@tanstack/react-query";
import {TooltipProvider} from "@/components/ui/tooltip.tsx";
import {RouterProvider} from "react-router/dom";
import {router} from "@/app/router.tsx";
import "@fontsource-variable/geist";
import {ThemeProvider} from "@/components/theme-provider";

const queryClient = new QueryClient({
    defaultOptions: {
        queries: {
            retry: 1,
            staleTime: 30_000
        }
    }
});

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ThemeProvider defaultTheme="light" storageKey="lids-theme">
            <QueryClientProvider client={queryClient}>
                <TooltipProvider>
                    <RouterProvider router={router}/>
                </TooltipProvider>
            </QueryClientProvider>
        </ThemeProvider>
    </StrictMode>,
)
