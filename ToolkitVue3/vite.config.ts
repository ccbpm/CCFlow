import { fileURLToPath, URL } from 'node:url'
import type { ConfigEnv } from 'vite'
import { loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'

// https://vitejs.dev/config/
export default ({ command, mode }: ConfigEnv) => {
  const root = process.cwd()
  const env = loadEnv(mode, root)
  const isBuild = command === 'build' // 可以根据开发 / 生产环境做区分
  const proxyConfig = {
    target: env.VITE_GLOB_APP_URL,
    changeOrigin: true,
    rewrite: (path: string) => path.replace(/^\/api/, '')
  }
  return {
    plugins: [vue(), vueJsx()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url))
      }
    },
    server: {
      proxy: {
        '/api': proxyConfig
      },
      open: true,
      port: 16384
    }
  }
}
