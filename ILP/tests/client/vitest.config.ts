import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import path from 'path'
import { fileURLToPath } from 'url'

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: true,
    environment: 'jsdom',
    include: ['unit/**/*.spec.{js,ts}']
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, '../src/client/ilp.client/src')
    }
  }
})
