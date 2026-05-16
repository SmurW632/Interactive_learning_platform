const isProduction = process.env.NODE_ENV === 'production'

// Локальная разработка
const API_BASE_URL = isProduction
  ? 'https://ilp-backend-re98.onrender.com/api/v1'  // Продакшен (Render)
  : 'http://localhost:5004/api/v1'                  // Локальный бэкенд

const AI_BASE_URL = isProduction
  ? 'https://alicagpt-qjgz.onrender.com'               // Продакшен (Render)
  : 'http://127.0.0.1:8001'                         // Локальный Python

export { AI_BASE_URL, API_BASE_URL }

