const { defineConfig } = require('@vue/cli-service')

module.exports = defineConfig({
  publicPath: process.env.NODE_ENV === 'production'
    ? '/Interactive_learning_platform/'
    : '/',
  outputDir: 'dist',
  assetsDir: 'assets',
  indexPath: 'index.html',
  devServer: {
    port: 54114,
    proxy: {
      '/api': {
        target: process.env.VUE_APP_API_URL || 'http://localhost:5004',
        changeOrigin: true,
      },
      '/swagger': {
        target: process.env.VUE_APP_API_URL || 'http://localhost:5004',
        changeOrigin: true,
      },
      '/research': {
        target: process.env.VUE_APP_API_URL || 'http://localhost:8001',
        changeOrigin: true,
      }
    },
    historyApiFallback: {
      index: '/hub.html'
    }
  }
})
