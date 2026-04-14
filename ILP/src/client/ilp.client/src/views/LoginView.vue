<template>
  <div class="container">
    <div class="card">
      <!-- Логотип -->
      <div class="logo">
        <span class="logo-emoji">📚</span>
        <span class="logo-gradient">EduPlatform</span>
      </div>

      <!-- Заголовок -->
      <h2>🔐 Вход в аккаунт</h2>

      <!-- Форма входа -->
      <form @submit.prevent="handleLogin">
        <!-- Почта -->
        <div class="form-group">
          <label for="email">Почта</label>
          <div class="input-wrapper">
            <span class="input-icon">📧</span>
            <input
              id="email"
              v-model="email"
              type="email"
              class="with-icon"
              placeholder="user@example.com"
              required
            />
          </div>
        </div>

        <!-- Пароль -->
        <div class="form-group">
          <label for="password">Пароль</label>
          <div class="input-wrapper">
            <span class="input-icon">🔒</span>
            <input
              id="password"
              v-model="password"
              :type="passwordFieldType"
              class="with-icon"
              placeholder="Введите пароль"
              required
            />
            <button type="button" class="password-toggle" @click="togglePassword">
              👁️
            </button>
          </div>
        </div>

        <!-- Чекбокс "Запомнить меня" -->
        <div style="margin-bottom: 24px">
          <label class="checkbox-wrapper">
            <input type="checkbox" v-model="rememberMe" /> Запомнить меня
          </label>
        </div>

        <!-- Кнопка входа -->
        <button type="submit" class="btn btn-primary" :disabled="loading">
          {{ loading ? 'Загрузка...' : 'Войти' }}
        </button>

        <!-- Сообщение об ошибке -->
        <div v-if="errorMessage" class="alert alert-error" style="margin-top: 16px">
          {{ errorMessage }}
        </div>
      </form>

      <!-- Ссылка на регистрацию -->
      <p style="text-align: center; margin-top: 24px; color: #666">
        Нет аккаунта? <router-link to="/register">Зарегистрироваться</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { useAuthStore } from '@/js/stores/auth'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const authStore = useAuthStore()

// Данные формы
const email = ref('')
const password = ref('')
const rememberMe = ref(false)
const loading = ref(false)
const errorMessage = ref('')
const passwordFieldType = ref('password')

// Показать/скрыть пароль
const togglePassword = () => {
  passwordFieldType.value = passwordFieldType.value === 'password' ? 'text' : 'password'
}

// Отправка формы
const handleLogin = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    await authStore.login(email.value, password.value, rememberMe.value)
    router.push('/profile')
  } catch (error) {
    errorMessage.value = error.response?.data?.message //|| 'Ошибка входа. Проверьте почту и пароль.'
  } finally {
    loading.value = false
  }
}
</script>
