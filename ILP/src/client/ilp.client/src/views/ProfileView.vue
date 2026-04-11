<template>
  <div class="profile-container">
    <h1>Личный кабинет</h1>
    <div v-if="authStore.user" class="profile-info">
      <p><strong>Имя:</strong> {{ authStore.user.firstName }} {{ authStore.user.lastName }}</p>
      <p><strong>Email:</strong> {{ authStore.user.email }}</p>
    </div>
    <p>Это защищенная страница</p>
    <button @click="logout" class="btn-logout">Выйти</button>
  </div>
</template>

<script setup>
import { useAuthStore } from '@/js/stores/auth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

const logout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.profile-container {
  max-width: 600px;
  margin: 50px auto;
  padding: 30px;
  background: white;
  border-radius: 20px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.profile-info {
  text-align: left;
  background: #f5f5f5;
  padding: 20px;
  border-radius: 10px;
  margin: 20px 0;
}

.btn-logout {
  padding: 10px 20px;
  background: #dc3545;
  color: white;
  border: none;
  border-radius: 10px;
  cursor: pointer;
  font-size: 16px;
}

.btn-logout:hover {
  background: #c82333;
}
</style>
