import axios from '@/js/utils/axios'
import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || sessionStorage.getItem('token') || null,
    user: null,
    loading: false
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    userName: (state) => {
      if (!state.user) return ''
      return `${state.user.firstName || ''} ${state.user.lastName || ''}`.trim()
    }
  },


  actions: {
    async login(email, password, rememberMe = false) {
      this.loading = true
      try {
        const response = await axios.post(`/Auth/login`, {
          email,
          password
        })

        const data = response.data
        this.token = data.token
        this.user = data.user

        if (rememberMe) {
          localStorage.setItem('token', data.token)
        } else {
          sessionStorage.setItem('token', data.token)
        }

        // Устанавливаем заголовок для axios
        axios.defaults.headers.common['Authorization'] = `Bearer ${data.token}`

        return true
      } catch (error) {
        console.error('Login error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async register(email, password, firstName, lastName) {
      this.loading = true
      try {
        const response = await axios.post(`/Auth/register`, {
          email,
          password,
          firstName,
          lastName
        })

        const data = response.data
        this.token = data.token
        this.user = data.user
        localStorage.setItem('token', data.token)

        // Устанавливаем заголовок для axios
        axios.defaults.headers.common['Authorization'] = `Bearer ${data.token}`

        return true
      } catch (error) {
        console.error('Register error:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    logout() {
      this.token = null
      this.user = null
      localStorage.removeItem('token')
      sessionStorage.removeItem('token')
      delete axios.defaults.headers.common['Authorization']
    },

    checkAuth() {
      const token = localStorage.getItem('token') || sessionStorage.getItem('token')
      if (token) {
        this.token = token
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
        return true
      }
      return false
    }
  }
})
