import { defineStore } from 'pinia'

export const useUserStore = defineStore('user', {
  state: () => ({
    profile: null
  }),

  getters: {
    userProfile: (state) => state.profile
  },

  actions: {
    setProfile(profile) {
      this.profile = profile
    },

    clearProfile() {
      this.profile = null
    }
  }
})
