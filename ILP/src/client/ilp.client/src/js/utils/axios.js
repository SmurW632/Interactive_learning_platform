import { useAuthStore } from '@/js/stores/user'
import axios from 'axios'

const axiosInstance = axios.create({
  baseURL: 'https://localhost:5001/api/v1',
  timeout: 10000
})

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
