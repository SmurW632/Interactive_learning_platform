
// // Вспомогательная функция для обработки ответов (единообразие)
// async function handleResponse(response) {
//   if (!response.ok) {
//     let errorMessage = `Ошибка HTTP: ${response.status}`
//     try {
//       const errorData = await response.json()
//       errorMessage = errorData.message || errorMessage
//     } catch {
//       // Если ответ не в JSON, оставляем стандартное сообщение
//     }
//     throw new Error(errorMessage)
//   }
//   // Если статус 204 (No Content), возвращаем null
//   if (response.status === 204) {
//     return null
//   }
//   return response.json()
// }

// export const authApi = {
//   // Исправлено: добавлена обработка ошибок и валидация
//   async login(email, password) {
//     if (!email || !password) {
//       throw new Error('Email и пароль обязательны для заполнения')
//     }

//     const response = await fetch(`${API_BASE}/login`, {
//       method: 'POST',
//       headers: { 'Content-Type': 'application/json' },
//       body: JSON.stringify({ email, password })
//     })
//     return handleResponse(response)
//   },

//   // Исправлено: добавлена валидация данных
//   async register(email, password, firstName, lastName) {
//     if (!email || !password || !firstName || !lastName) {
//       throw new Error('Все поля обязательны для заполнения')
//     }
//     if (password.length < 6) {
//       throw new Error('Пароль должен содержать минимум 6 символов')
//     }

//     const response = await fetch(`${API_BASE}/register`, {
//       method: 'POST',
//       headers: { 'Content-Type': 'application/json' },
//       body: JSON.stringify({
//         email,
//         password,
//         first_name: firstName,
//         last_name: lastName
//       })
//     })
//     return handleResponse(response)
//   },

//   // Исправлено: используется базовая переменная API_BASE
//   async updateProfile(userId, userData) {
//     if (!userId || !userData) {
//       throw new Error('ID пользователя и данные для обновления обязательны')
//     }

//     const response = await fetch(`${API_BASE}/users/${userId}`, {
//       method: 'PUT',
//       headers: { 'Content-Type': 'application/json' },
//       body: JSON.stringify(userData)
//     })
//     return handleResponse(response)
//   },

//   // Исправлено: защита от дублирующихся запросов
//   getCurrentUser: (() => {
//     let pendingPromise = null
//     return async () => {
//       if (pendingPromise) {
//         return pendingPromise
//       }
//       pendingPromise = (async () => {
//         const response = await fetch(`${API_BASE}/me`)
//         return handleResponse(response)
//       })()
//       try {
//         return await pendingPromise
//       } finally {
//         pendingPromise = null
//       }
//     }
//   })(),

//   // Исправлено: обработка 401 и единообразное возвращаемое значение
//   async deleteAccount() {
//     const response = await fetch(`${API_BASE}/account`, {
//       method: 'DELETE'
//     })

//     if (response.status === 401) {
//       throw new Error('UNAUTHORIZED')
//     }
//     return handleResponse(response)
//   },

//   // Исправлено: проверка данных перед отправкой
//   async changePassword(oldPassword, newPassword) {
//     if (!oldPassword || !newPassword) {
//       throw new Error('Старый и новый пароль обязательны')
//     }
//     if (newPassword.length < 6) {
//       throw new Error('Новый пароль должен содержать минимум 6 символов')
//     }
//     if (oldPassword === newPassword) {
//       throw new Error('Новый пароль должен отличаться от старого')
//     }

//     const response = await fetch(`${API_BASE}/change-password`, {
//       method: 'POST',
//       headers: { 'Content-Type': 'application/json' },
//       body: JSON.stringify({ oldPassword, newPassword })
//     })
//     return handleResponse(response)
//   }
// }
