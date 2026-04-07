const API_BASE = '/api/v1/auth'

export const authApi = {
  async login(email, password) {
    const response = await fetch(`${API_BASE}/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password })
    })
    return response.json()
  },

  async register(email, password, firstName, lastName) {
    const response = await fetch(`${API_BASE}/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email,
        password,
        first_name: firstName,
        last_name: lastName
      })
    })
    return response.json()
  },

  async updateProfile(userId, userData) {
    const response = await fetch(`/api/v1/users/${userId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(userData)
    })
    return response.json()
  },

  async getCurrentUser() {
    const response = await fetch(`${API_BASE}/me`)
    return response.json()
  },

  async deleteAccount() {
    const response = await fetch(`${API_BASE}/account`, {
      method: 'DELETE'
    })
    return response.ok
  },

  async changePassword(oldPassword, newPassword) {
    const response = await fetch(`${API_BASE}/change-password`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ oldPassword, newPassword })
    })
    return response.json()
  }
}
