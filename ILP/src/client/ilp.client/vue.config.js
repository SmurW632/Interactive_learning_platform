const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  devServer: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5005',
        changeOrigin: true
      }
    }
  },
  transpileDependencies: true
})
