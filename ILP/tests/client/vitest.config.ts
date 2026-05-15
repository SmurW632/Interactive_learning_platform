import { defineConfig } from 'vitest/config'

export default defineConfig({
  test: {
    globals: true,
    environment: 'node',
    include: ['**/*.test.ts', '**/*.spec.ts', 'units/**/*.test.ts', 'units/**/*.spec.ts'],
    testTimeout: 10000,
  },
})
