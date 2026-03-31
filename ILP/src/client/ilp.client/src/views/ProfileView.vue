<template>
  <div>
    <h1>Личный кабинет</h1>
    <p>Это защищенная страница</p>
    <button @click="logout">Выйти</button>
  </div>
</template>

<script setup>
import { useUserStore } from '@/stores/user'
import { useRouter } from 'vue-router'

const userStore = useUserStore()
const router = useRouter()

const logout = () => {
  userStore.logout()
  router.push('/login')
}
</script>
