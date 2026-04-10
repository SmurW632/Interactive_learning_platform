import { createPinia } from 'pinia'
import { createApp } from 'vue'
import App from './App.vue'
import './assets/styles/main.css'
import router from './router'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia)
app.use(router)

// Инициализация auth store после монтирования pinia
import { useAuthStore } from './stores/auth'
const authStore = useAuthStore()
authStore.checkAuth()

app.mount('#app')
