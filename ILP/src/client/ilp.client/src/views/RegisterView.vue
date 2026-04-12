<template>
  <div class="container">
    <div class="card">
      <!-- Кнопка назад -->
      <router-link to="/login" class="back-link">← Назад</router-link>

      <!-- Логотип -->
      <div class="logo">
        <span class="logo-emoji">📚</span>
        <span class="logo-gradient">EduPlatform</span>
      </div>

      <!-- Заголовок -->
      <h2>✨ Регистрация</h2>

      <!-- Форма регистрации -->
      <form @submit.prevent="handleRegister">
        <!-- Имя и Фамилия в сетке -->
        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px">
          <div class="form-group">
            <label for="firstName">Имя</label>
            <div class="input-wrapper">
              <span class="input-icon">👤</span>
              <input
                id="firstName"
                v-model="firstName"
                type="text"
                class="with-icon"
                placeholder="Иван"
                required
              />
            </div>
          </div>

          <div class="form-group">
            <label for="lastName">Фамилия</label>
            <div class="input-wrapper">
              <span class="input-icon">👤</span>
              <input
                id="lastName"
                v-model="lastName"
                type="text"
                class="with-icon"
                placeholder="Петров"
                required
              />
            </div>
          </div>
        </div>

        <!-- Email -->
        <div class="form-group">
          <label for="email">Почта</label>
          <div class="input-wrapper">
            <span class="input-icon">📧</span>
            <input
              id="email"
              v-model="email"
              type="email"
              class="with-icon"
              placeholder="ivan.petrov@example.com"
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
              placeholder="Создайте пароль"
              @input="checkPasswordStrength"
              required
            />
            <button type="button" class="password-toggle" @click="togglePassword">
              👁️
            </button>
          </div>

          <!-- Индикатор силы пароля -->
          <div class="password-strength">
            <div
              class="password-strength-bar"
              :style="{ width: strengthPercent + '%', background: strengthColor }"
            ></div>
          </div>
          <div class="input-hint">Пароль должен содержать минимум 8 символов</div>

          <!-- Требования к паролю -->
          <div class="password-requirements">
            <div class="requirement" :class="{ met: hasLength }">
              {{ hasLength ? '✅' : '❌' }} Минимум 8 символов
            </div>
            <div class="requirement" :class="{ met: hasNumber }">
              {{ hasNumber ? '✅' : '❌' }} Хотя бы одна цифра
            </div>
            <div class="requirement" :class="{ met: hasLower }">
              {{ hasLower ? '✅' : '❌' }} Строчная буква
            </div>
            <div class="requirement" :class="{ met: hasUpper }">
              {{ hasUpper ? '✅' : '❌' }} Заглавная буква
            </div>
          </div>
        </div>

        <!-- Подтверждение пароля -->
        <div class="form-group">
          <label for="confirmPassword">Подтвердите пароль</label>
          <div class="input-wrapper">
            <span class="input-icon">🔒</span>
            <input
              id="confirmPassword"
              v-model="confirmPassword"
              type="password"
              class="with-icon"
              placeholder="Повторите пароль"
              @input="checkPasswordMatch"
              required
            />
          </div>
          <div class="error-message" v-if="passwordMismatch">
            ❌ Пароли не совпадают
          </div>
        </div>

        <!-- Кнопка регистрации -->
        <button type="submit" class="btn btn-primary" :disabled="!isFormValid || loading">
          {{ loading ? 'Загрузка...' : 'Зарегистрироваться' }}
        </button>

        <!-- Сообщение об ошибке -->
        <div v-if="errorMessage" class="alert alert-error" style="margin-top: 16px">
          {{ errorMessage }}
        </div>
      </form>

      <!-- Ссылка на вход -->
      <p style="text-align: center; margin-top: 24px; color: var(--text-secondary)">
        Уже есть аккаунт? <router-link to="/login">Войти</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { useAuthStore } from '@/js/stores/auth'
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const authStore = useAuthStore()

// Данные формы
const firstName = ref('')
const lastName = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const errorMessage = ref('')
const passwordFieldType = ref('password')

// Валидация пароля
const hasLength = computed(() => password.value.length >= 8)
const hasNumber = computed(() => /\d/.test(password.value))
const hasLower = computed(() => /[a-z]/.test(password.value))
const hasUpper = computed(() => /[A-Z]/.test(password.value))
const passwordMismatch = computed(() => password.value !== confirmPassword.value && confirmPassword.value !== '')

// Сила пароля
const strengthPercent = computed(() => {
  const strength = [hasLength.value, hasNumber.value, hasLower.value, hasUpper.value].filter(Boolean).length
  return (strength / 4) * 100
})

const strengthColor = computed(() => {
  const strength = [hasLength.value, hasNumber.value, hasLower.value, hasUpper.value].filter(Boolean).length
  if (strength <= 1) return '#EF4444'
  if (strength <= 2) return '#F59E0B'
  if (strength <= 3) return '#3B82F6'
  return '#10B981'
})

// Форма валидна?
const isFormValid = computed(() => {
  return (
    hasLength.value &&
    hasNumber.value &&
    hasLower.value &&
    hasUpper.value &&
    password.value === confirmPassword.value &&
    email.value &&
    firstName.value &&
    lastName.value
  )
})

// Методы
const togglePassword = () => {
  passwordFieldType.value = passwordFieldType.value === 'password' ? 'text' : 'password'
}

const checkPasswordStrength = () => {
  // вычисляется автоматически через computed
}

const checkPasswordMatch = () => {
  // вычисляется автоматически через computed
}

// Отправка формы
const handleRegister = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    await authStore.register(email.value, password.value, firstName.value, lastName.value)
    router.push('/')
  } catch (error) {
    errorMessage.value = error.message || 'Ошибка регистрации. Попробуйте другой email.'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.password-strength {
  margin-top: 8px;
  height: 4px;
  background: var(--border);
  border-radius: 2px;
  overflow: hidden;
}

.password-strength-bar {
  height: 100%;
  transition: width 0.3s, background 0.3s;
}

.password-requirements {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 8px;
  margin-top: 12px;
  font-size: 12px;
  color: var(--text-secondary);
}

.requirement {
  display: flex;
  align-items: center;
  gap: 4px;
}

.requirement.met {
  color: var(--success);
}
</style>
