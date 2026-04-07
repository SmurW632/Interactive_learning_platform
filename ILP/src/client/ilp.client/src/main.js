import { createApp } from 'vue'
import { createPinia } from 'pinia'
import App from './App.vue'
import router from './router'
import './assets/styles/main.css'

const app = createApp(App)

app.use(createPinia())  // подключаем хранилище
app.use(router)         // подключаем маршрутизацию

app.mount('#app')
