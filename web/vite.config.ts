import react from '@vitejs/plugin-react'
import {defineConfig, loadEnv} from 'vite'
import tailwindcss from "@tailwindcss/vite";
import {fileURLToPath} from "node:url";

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "")

  return {
    plugins: [react(), tailwindcss()],
    server: {
      host: "127.0.0.1",
      port: 5173,
      strictPort: true,

      proxy: {
        "/api": {
          target: env.API_PROXY_TARGET,
          changeOrigin: true,
          secure: false
        }
      }
    },
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url)),
      }
    }
  }
})
