import { createPinia } from 'pinia'
import { createApp } from 'vue'
import App from './App.vue'
import './assets/styles/main.css'
import router from './js/router'
import axios from './js/utils/axios'

window.axios = axios

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)

app.config.globalProperties.$axios = axios

// Инициализация auth store после монтирования pinia
import { useAuthStore } from './js/stores/auth'
const authStore = useAuthStore()
authStore.checkAuth()

app.mount('#app')
