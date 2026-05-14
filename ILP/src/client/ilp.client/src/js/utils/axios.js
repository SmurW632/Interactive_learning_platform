import { useAuthStore } from '@/js/stores/user'
import axios from 'axios'

//const API_URL = process.env.VUE_APP_API_URL || 'http://localhost:5004'

// const isProduction = window.location.hostname !== 'localhost'
// const baseURL = isProduction
//   ? 'https://ilp-backend-re98.onrender.com/api/v1'   // ✅ URL вашего бэкенда
//   : '/api/v1'


const axiosInstance = axios.create({
  baseURL: 'https://ilp-backend-re98.onrender.com/api/v1',  // для локальной разработки используем прокси
  timeout: 30000
})



// const axiosInstance = axios.create({
//   baseURL: `${API_URL}/api/v1`,
//   timeout: 10000
// })

// Interceptor для обработки ошибок авторизации
axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      const authStore = useAuthStore()
      authStore.logout()
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

export default axiosInstance
