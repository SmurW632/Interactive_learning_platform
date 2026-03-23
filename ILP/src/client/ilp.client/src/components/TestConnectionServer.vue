<template>
  <div>
    <h1>Тест связи с сервером</h1>
    <button @click="testConnection">Проверить</button>
    <p v-if="result">{{ result }}</p>
  </div>
</template>

<script setup>
import { ref } from "vue";

const result = ref("");

const testConnection = async () => {
  try {
    const response = await fetch("/api/v1/Test/connection");
    const data = await response.json();
    result.value = data.message;
  } catch (error) {
    result.value = "Ошибка: " + error.message;
  }
};
</script>
