const { defineConfig } = require('@vue/cli-service')

module.exports = defineConfig({
  publicPath: process.env.NODE_ENV === 'production'
    ? '/Interactive_learning_platform/'
    : '/',
  outputDir: 'dist',
  assetsDir: 'assets',
  indexPath: 'index.html',
  devServer: {
    port: 3001,
    proxy: {
      '/api': {
        target: 'http://localhost:5004',
        changeOrigin: true,
  }
})
