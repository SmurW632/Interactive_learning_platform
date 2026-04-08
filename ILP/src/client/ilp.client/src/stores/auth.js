import { defineStore } from 'pinia'
import { authApi } from '@/api/auth'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    user: null,
    loading: false
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    userName: (state) => {
      if (!state.user) return ''
      return `${state.user.first_name || ''} ${state.user.last_name || ''}`.trim()
    }
  },

  actions: {
    async login(email, password, rememberMe = false) {
      this.loading = true
      try {
        const data = await authApi.login(email, password)
        this.token = data.token
        this.user = data.user

        if (rememberMe) {
          localStorage.setItem('token', data.token)
        }
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
        const data = await authApi.register(email, password, firstName, lastName)
        this.token = data.token
        this.user = data.user
        localStorage.setItem('token', data.token)
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
    }
  }
})
