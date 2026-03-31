<template>
  <div>
    <h1>Вход в систему</h1>
    <button @click="login">Войти</button>
  </div>
</template>

<script setup>
import { useUserStore } from '@/stores/user'
import { useRouter } from 'vue-router'

const userStore = useUserStore()
const router = useRouter()

const login = () => {
  userStore.setToken('fake-token-123')
  router.push('/profile')
}
</script>
