<template>
  <div class="container container-wide">
    <!-- Шапка (оставляем без изменений) -->
    <header class="header">
      <div class="header-logo">
        <span class="logo-emoji">📚</span>
        <span class="logo-text">EduPlatform</span>
      </div>
      <nav class="header-nav">
        <router-link to="/catalog" class="active">Каталог</router-link>
        <router-link to="/my-courses">Мои курсы</router-link>
        <a href="#">О нас</a>
      </nav>
      <div class="header-actions" v-if="!isAuthenticated">
        <router-link to="/login" class="btn btn-text">Вход</router-link>
        <router-link to="/register" class="btn btn-primary btn-sm">Регистрация</router-link>
      </div>
      <div class="header-actions" v-else>
        <span class="user-name">{{ userName }}</span>
        <button @click="logout" class="btn btn-text">Выйти</button>
      </div>
    </header>

    <!-- Поиск и фильтры -->
    <div class="search-section">
      <div class="search-bar">
        <div class="search-input-wrapper">
          <span class="input-icon">🔍</span>
          <input
            type="text"
            v-model="localFilters.search"
            @input="onSearchChange"
            placeholder="Поиск курсов по названию, автору или теме..."
          />
        </div>
        <button class="search-btn" @click="applyFilters">Найти</button>
      </div>

      <div class="filters">
        <select class="filter-select" v-model="localFilters.category" @change="applyFilters">
          <option value="">Все категории</option>
          <option v-for="cat in coursesStore.categories" :key="cat.id" :value="cat.name">
            {{ cat.name }}
          </option>
        </select>

        <select class="filter-select" v-model="localFilters.level" @change="applyFilters">
          <option value="">Любой уровень</option>
          <option v-for="level in levels" :key="level.value" :value="level.value">
            {{ level.label }}
          </option>
        </select>

        <select class="filter-select" v-model="localFilters.priceRange" @change="applyFilters">
          <option value="all">Все цены</option>
          <option value="free">Бесплатные</option>
          <option value="paid">Платные</option>
        </select>

        <select class="filter-select" v-model="localFilters.duration" @change="applyFilters">
          <option value="all">Любая длительность</option>
          <option value="0-5">До 5 часов</option>
          <option value="5-10">5-10 часов</option>
          <option value="10-20">10-20 часов</option>
          <option value="20">Более 20 часов</option>
        </select>
      </div>

      <!-- Активные теги фильтров -->
      <div class="filter-tags" v-if="hasActiveFilters">
        <span v-if="localFilters.search" class="filter-tag" @click="removeFilter('search')">
          Поиск: {{ localFilters.search }} <span class="remove">✕</span>
        </span>
        <span v-if="localFilters.category" class="filter-tag" @click="removeFilter('category')">
          Категория: {{ localFilters.category }} <span class="remove">✕</span>
        </span>
        <span v-if="localFilters.level" class="filter-tag" @click="removeFilter('level')">
          {{ getLevelLabel(localFilters.level) }} <span class="remove">✕</span>
        </span>
        <span v-if="localFilters.priceRange === 'free'" class="filter-tag" @click="removeFilter('priceRange')">
          Бесплатные <span class="remove">✕</span>
        </span>
        <span v-if="localFilters.priceRange === 'paid'" class="filter-tag" @click="removeFilter('priceRange')">
          Платные <span class="remove">✕</span>
        </span>
        <span v-if="localFilters.duration !== 'all'" class="filter-tag" @click="removeFilter('duration')">
          {{ getDurationLabel(localFilters.duration) }} <span class="remove">✕</span>
        </span>
        <button class="filter-tag reset-btn" @click="resetAllFilters">
          Сбросить все ✕
        </button>
      </div>
    </div>

    <!-- Результаты -->
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px;">
      <h2 style="font-size: 28px;">Найдено {{ coursesStore.totalCount }} курсов</h2>
      <div style="display: flex; gap: 16px; align-items: center;">
        <span style="color: var(--text-secondary);">Сортировка:</span>
        <select
          style="border: 1px solid var(--border); padding: 8px 16px; border-radius: 8px; background: white;"
          v-model="localSortBy"
          @change="onSortChange"
        >
          <option value="popular">Сначала популярные</option>
          <option value="newest">Сначала новые</option>
          <option value="price_asc">Сначала дешёвые</option>
          <option value="price_desc">Сначала дорогие</option>
          <option value="rating">По рейтингу</option>
        </select>
      </div>
    </div>

    <!-- Скелетон загрузки -->
    <div v-if="coursesStore.loading" class="catalog-grid">
      <div v-for="i in 6" :key="i" class="course-card skeleton">
        <div class="course-card-image skeleton-shimmer"></div>
        <div class="course-card-content">
          <div class="skeleton-title"></div>
          <div class="skeleton-text"></div>
          <div class="skeleton-text short"></div>
        </div>
      </div>
    </div>

    <!-- Сетка курсов -->
    <div v-else class="catalog-grid">
      <div
        v-for="course in coursesStore.courses"
        :key="course.id"
        class="course-card"
        @click="goToCourse(course.id)"
      >
        <div class="course-card-image">
          <span>{{ getCourseEmoji(course.title) }}</span>
        </div>
        <div class="course-card-content">
          <h3 class="course-card-title">{{ course.title }}</h3>
          <div class="course-card-author">{{ course.author }}</div>

          <div class="course-meta-icons">
            <span>📚 {{ course.lessonsCount || 0 }} уроков · ⏱️ {{ course.durationHours }} часов</span>
            <span>👥 {{ formatNumber(course.totalReviews) }} учеников</span>
          </div>

          <div class="course-card-rating">
            <span class="stars">{{ getStars(course.averageRating) }}</span>
            <span class="rating-value">{{ course.averageRating.toFixed(1) }}</span>
            <span class="reviews-count">({{ formatNumber(course.totalReviews) }} отзывов)</span>
          </div>

          <div class="course-description-preview">
            {{ course.shortDescription }}
          </div>

          <div class="course-card-footer">
            <span class="course-level" :class="getLevelClass(course.level)">
              {{ getLevelIcon(course.level) }} {{ getLevelLabel(course.level) }}
            </span>
            <span class="course-price" :class="{ free: course.isFree }">
              {{ course.isFree ? 'Бесплатно' : formatPrice(course.price) }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Пагинация -->
    <div class="pagination" v-if="coursesStore.totalPages > 1">
      <span
        class="pagination-item"
        :class="{ disabled: coursesStore.currentPage === 1 }"
        @click="changePage(coursesStore.currentPage - 1)"
      >←</span>

      <span
        v-for="page in visiblePages"
        :key="page"
        class="pagination-item"
        :class="{ active: page === coursesStore.currentPage }"
        @click="changePage(page)"
      >{{ page }}</span>

      <span
        class="pagination-item"
        :class="{ disabled: coursesStore.currentPage === coursesStore.totalPages }"
        @click="changePage(coursesStore.currentPage + 1)"
      >→</span>
    </div>
  </div>
</template>

<script setup>
import { useAuthStore } from '@/js/stores/auth'
import { useCoursesStore } from '@/js/stores/courses'
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const authStore = useAuthStore()
const coursesStore = useCoursesStore()

// Локальные копии фильтров для двустороннего связывания
const localFilters = ref({
  search: '',
  category: '',
  level: '',
  priceRange: 'all',
  duration: 'all'
})

const localSortBy = ref('popular')

// Уровни сложности (можно вынести в отдельный файл или получать с сервера)
const levels = [
  { value: 'Beginner', label: 'Начальный' },
  { value: 'Intermediate', label: 'Средний' },
  { value: 'Advanced', label: 'Продвинутый' }
]

// Вычисляемые свойства
const hasActiveFilters = computed(() => {
  return localFilters.value.search ||
         localFilters.value.category ||
         localFilters.value.level ||
         localFilters.value.priceRange !== 'all' ||
         localFilters.value.duration !== 'all'
})

// Аутентификация
const isAuthenticated = computed(() => authStore.isAuthenticated)
const userName = computed(() => authStore.userName)

// Пагинация: какие страницы показывать
const visiblePages = computed(() => {
  const pages = []
  const maxVisible = 5
  const current = coursesStore.currentPage
  const total = coursesStore.totalPages

  let start = Math.max(1, current - Math.floor(maxVisible / 2))
  let end = Math.min(total, start + maxVisible - 1)

  if (end - start + 1 < maxVisible) {
    start = Math.max(1, end - maxVisible + 1)
  }

  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  return pages
})

// Методы
const applyFilters = () => {
  coursesStore.setFilter('search', localFilters.value.search)
  coursesStore.setFilter('category', localFilters.value.category)
  coursesStore.setFilter('level', localFilters.value.level)
  coursesStore.setFilter('priceRange', localFilters.value.priceRange)
  coursesStore.setFilter('duration', localFilters.value.duration)
}

const onSearchChange = () => {
  // Debounce для поиска
  clearTimeout(window.searchTimeout)
  window.searchTimeout = setTimeout(() => {
    applyFilters()
  }, 500)
}

const onSortChange = () => {
  coursesStore.setSortBy(localSortBy.value)
}

const removeFilter = (key) => {
  if (key === 'search') {
    localFilters.value.search = ''
  } else if (key === 'category') {
    localFilters.value.category = ''
  } else if (key === 'level') {
    localFilters.value.level = ''
  } else if (key === 'priceRange') {
    localFilters.value.priceRange = 'all'
  } else if (key === 'duration') {
    localFilters.value.duration = 'all'
  }
  applyFilters()
}

const resetAllFilters = () => {
  localFilters.value = {
    search: '',
    category: '',
    level: '',
    priceRange: 'all',
    duration: 'all'
  }
  localSortBy.value = 'popular'
  coursesStore.resetFilters()
}

const changePage = (page) => {
  coursesStore.setPage(page)
}

const goToCourse = (courseId) => {
  router.push(`/course/${courseId}`)
}

const logout = () => {
  authStore.logout()
  router.push('/')
}

// Вспомогательные функции для отображения
const formatNumber = (num) => {
  if (num >= 1000) {
    return (num / 1000).toFixed(1) + 'k'
  }
  return num.toString()
}

const formatPrice = (price) => {
  return new Intl.NumberFormat('ru-RU').format(price) + ' ₽'
}

const getStars = (rating) => {
  const full = Math.floor(rating)
  const half = rating % 1 >= 0.5
  const empty = 5 - full - (half ? 1 : 0)
  return '★'.repeat(full) + (half ? '½' : '') + '☆'.repeat(empty)
}

const getLevelLabel = (level) => {
  const map = {
    Beginner: 'Начальный',
    Intermediate: 'Средний',
    Advanced: 'Продвинутый'
  }
  return map[level] || level
}

const getLevelIcon = (level) => {
  const map = {
    Beginner: '🟢',
    Intermediate: '🟡',
    Advanced: '🔴'
  }
  return map[level] || '🔵'
}

const getLevelClass = (level) => {
  const map = {
    Beginner: 'level-beginner',
    Intermediate: 'level-intermediate',
    Advanced: 'level-advanced'
  }
  return map[level] || ''
}

const getDurationLabel = (duration) => {
  const map = {
    '0-5': 'До 5 часов',
    '5-10': '5-10 часов',
    '10-20': '10-20 часов',
    '20': 'Более 20 часов'
  }
  return map[duration] || duration
}

const getCourseEmoji = (title) => {
  const emojiMap = {
    'JavaScript': '📘',
    'Python': '🐍',
    'Дизайн': '🎨',
    'Excel': '📊',
    'React': '⚛️',
    'SQL': '🗄️',
    'Machine': '🤖',
    'Английский': '📱'
  }
  for (const [key, emoji] of Object.entries(emojiMap)) {
    if (title.includes(key)) return emoji
  }
  return '📚'
}

// Загрузка данных при монтировании
onMounted(async () => {
  await Promise.all([
    coursesStore.fetchCourses(),
    coursesStore.fetchCategories()
  ])
})
</script>

<style scoped>
/* Стили остаются без изменений */
.skeleton .course-card-image {
  background: linear-gradient(135deg, #e0e0e0 0%, #f0f0f0 100%);
}

.skeleton-shimmer {
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

.skeleton-title {
  height: 24px;
  background: #e0e0e0;
  border-radius: 4px;
  margin-bottom: 12px;
  width: 80%;
}

.skeleton-text {
  height: 16px;
  background: #e0e0e0;
  border-radius: 4px;
  margin-bottom: 8px;
}

.skeleton-text.short {
  width: 60%;
}

.level-beginner {
  background: #D1FAE5;
  color: #065F46;
}

.level-intermediate {
  background: #FEF3C7;
  color: #92400E;
}

.level-advanced {
  background: #FEE2E2;
  color: #991B1B;
}

.reset-btn {
  background: #ef4444 !important;
  color: white !important;
  outline: none !important;
  box-shadow: none !important;
  border: none !important;
}

.reset-btn:hover {
  background: #dc2626 !important;
  color: white !important;
}

.user-name {
  font-weight: 500;
  color: #333;
}

.course-card-content {
  text-align: left !important;
}

.course-card-title {
  text-align: left !important;
  justify-content: flex-start !important;
}

.catalog-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
  gap: 24px;
  margin-bottom: 40px;
}

.course-card {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  transition: transform 0.2s, box-shadow 0.2s;
  cursor: pointer;
}

.course-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

.course-card-image {
  height: 160px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 64px;
}

.course-card-content {
  padding: 20px;
}

.course-card-title {
  font-size: 18px;
  font-weight: 600;
  margin-bottom: 8px;
  color: #333;
}

.course-card-author {
  font-size: 14px;
  color: #666;
  margin-bottom: 12px;
}

.course-meta-icons {
  display: flex;
  gap: 16px;
  font-size: 13px;
  color: #888;
  margin-bottom: 12px;
}

.course-card-rating {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
}

.stars {
  color: #fbbf24;
  font-size: 14px;
}

.rating-value {
  font-weight: 600;
  color: #333;
}

.reviews-count {
  font-size: 13px;
  color: #888;
}

.course-description-preview {
  font-size: 14px;
  color: #666;
  line-height: 1.5;
  margin-bottom: 16px;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.course-card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.course-level {
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 500;
}

.course-price {
  font-size: 18px;
  font-weight: 700;
  color: #667eea;
}

.course-price.free {
  color: #10b981;
}

.pagination {
  display: flex;
  justify-content: center;
  gap: 8px;
  margin-top: 40px;
}

.pagination-item {
  padding: 8px 16px;
  border: 1px solid #e5e7eb;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s;
}

.pagination-item:hover:not(.disabled):not(.active) {
  background: #f3f4f6;
}

.pagination-item.active {
  background: #667eea;
  color: white;
  border-color: #667eea;
}

.pagination-item.disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
