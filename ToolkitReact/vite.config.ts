import react from "@vitejs/plugin-react-swc";
import { fileURLToPath, URL } from "node:url";
import type { ConfigEnv } from "vite";
import { loadEnv } from "vite";

// https://vitejs.dev/config/
export default ({ command, mode }: ConfigEnv) => {
  const root = process.cwd();
  const env = loadEnv(mode, root);
  const isBuild = command === "build"; // 可以根据开发 / 生产环境做区分
  const proxyConfig = {
    target: env.VITE_GLOB_APP_URL,
    changeOrigin: true,
    rewrite: (path: string) => path.replace(/^\/api/, ""),
  };
  return {
    plugins: [react()],
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url)),
      },
    },
    server: {
      proxy: {
        "/api": proxyConfig,
      },
      open: true,
      port: 5173,
    },
  };
};
