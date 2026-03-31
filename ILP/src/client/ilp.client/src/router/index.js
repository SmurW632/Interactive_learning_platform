import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/HomeView.vue')
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('../views/LoginView.vue')
  },
  {
    path: '/profile',
    name: 'Profile',
    component: () => import('../views/ProfileView.vue'),
    meta: { requiresAuth: true } // защищенный маршрут
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Защита маршрутов (AC4)
router.beforeEach((to, from, next) => {
  const isAuthenticated = localStorage.getItem('token') // проверка токена
  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login') // редирект на логин
  } else {
    next()
  }
})

export default router
