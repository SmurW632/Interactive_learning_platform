<template>
  <div class="research-view">
    <h1>Поиск материалов (YandexGPT)</h1>

    <div class="form">
      <label>
        Запрос:
        <textarea
          v-model="query"
          rows="4"
          placeholder="Например: Найди свежие материалы про AI agents и web search в LLM"
        ></textarea>
      </label>

      <button @click="send" :disabled="loading || !query.trim()">
        {{ loading ? "Идёт поиск..." : "Искать" }}
      </button>

      <p v-if="error" class="error">{{ error }}</p>
    </div>

    <div class="result" v-if="result">
      <h2>Результат</h2>
      <pre>{{ result }}</pre>
    </div>
  </div>
</template>

<script setup>
import { AI_BASE_URL } from '@/js/api/api'
import axios from '@/js/utils/axios'
import { ref } from 'vue'

const query = ref('Найди свежие материалы про AI agents и web search в LLM')
const result = ref('')
const error = ref('')
const loading = ref(false)

async function send() {
  loading.value = true
  error.value = ''
  result.value = ''

  try {
    // Прямой запрос к Python сервису (без прокси)
    const response = await axios.post(`${AI_BASE_URL}/research/search`, {
      query: query.value
    })

    result.value = response.data.result ?? ''
  } catch (e) {
    error.value = e.response?.data?.message || e.message || 'Ошибка запроса'
    console.error('Research error:', e)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.research-view {
  max-width: 800px;
  margin: 2rem auto;
  padding: 1.5rem;
  border-radius: 8px;
  background: #f7f7f7;
}

.form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

textarea {
  width: 100%;
  padding: 0.5rem;
  resize: vertical;
}

button {
  align-self: flex-start;
  padding: 0.5rem 1rem;
}

.error {
  color: #c00;
}

.result {
  margin-top: 2rem;
}

.result pre {
  white-space: pre-wrap;
  background: #111;
  color: #eee;
  padding: 1rem;
  border-radius: 6px;
}
</style>
