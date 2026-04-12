<template>
  <div class="container container-wide">
    <!-- Шапка -->
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
            v-model="filters.search"
            @input="onSearchChange"
            placeholder="Поиск курсов по названию, автору или теме..."
          >
        </div>
        <button class="search-btn" @click="applyFilters">Найти</button>
      </div>

    <div class="filters">
      <select class="filter-select" v-model="filters.category" @change="applyFilters">
        <option value="">Все категории</option>
        <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
      </select>

      <select class="filter-select" v-model="filters.level" @change="applyFilters">
        <option value="">Любой уровень</option>
        <option v-for="level in levels" :key="level.value" :value="level.value">
          {{ level.label }}
        </option>
      </select>

      <select class="filter-select" v-model="filters.priceRange" @change="applyFilters">
        <option value="all">Все цены</option>
        <option value="free">Бесплатные</option>
        <option value="paid">Платные</option>
      </select>

      <select class="filter-select" v-model="filters.duration" @change="applyFilters">
        <option value="all">Любая длительность</option>
        <option value="0-5">До 5 часов</option>
        <option value="5-10">5-10 часов</option>
        <option value="10-20">10-20 часов</option>
        <option value="20">Более 20 часов</option>
      </select>
    </div>

      <!-- Активные теги фильтров -->
      <div class="filter-tags" v-if="hasActiveFilters">
        <span
          v-if="filters.search"
          class="filter-tag"
          @click="removeFilter('search')"
        >
          Поиск: {{ filters.search }} <span class="remove">✕</span>
        </span>
        <span
          v-if="filters.category"
          class="filter-tag"
          @click="removeFilter('category')"
        >
          {{ getLevelLabel(filters.category) }} <span class="remove">✕</span>
        </span>
        <span
          v-if="filters.level"
          class="filter-tag"
          @click="removeFilter('level')"
        >
          {{ getLevelLabel(filters.level) }} <span class="remove">✕</span>
        </span>
        <span
          v-if="filters.priceRange === 'free'"
          class="filter-tag"
          @click="removeFilter('priceRange')"
        >
          Бесплатные <span class="remove">✕</span>
        </span>
        <span
          v-if="filters.priceRange === 'paid'"
          class="filter-tag"
          @click="removeFilter('priceRange')"
        >
          Платные <span class="remove">✕</span>
        </span>
        <span
          v-if="filters.duration !== 'all'"
          class="filter-tag"
          @click="removeFilter('duration')"
        >
          {{ getDurationLabel(filters.duration) }} <span class="remove">✕</span>
        </span>
        <button class="filter-tag reset-btn" @click="resetAllFilters">
          Сбросить все ✕
        </button>
      </div>
    </div>

    <!-- Результаты -->
    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px;">
      <h2 style="font-size: 28px;">Найдено {{ filteredCourses.length }} курсов</h2>
      <div style="display: flex; gap: 16px; align-items: center;">
        <span style="color: var(--text-secondary);">Сортировка:</span>
        <select
          style="border: 1px solid var(--border); padding: 8px 16px; border-radius: 8px; background: white;"
          v-model="sortBy"
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
    <div v-if="loading" class="catalog-grid">
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
        v-for="course in paginatedCourses"
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
            <span class="rating-value">{{ course.averageRating }}</span>
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
    <div class="pagination" v-if="totalPages > 1">
      <span
        class="pagination-item"
        :class="{ disabled: currentPage === 1 }"
        @click="changePage(currentPage - 1)"
      >←</span>

      <span
        v-for="page in visiblePages"
        :key="page"
        class="pagination-item"
        :class="{ active: page === currentPage }"
        @click="changePage(page)"
      >{{ page }}</span>

      <span
        class="pagination-item"
        :class="{ disabled: currentPage === totalPages }"
        @click="changePage(currentPage + 1)"
      >→</span>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/js/stores/auth'
import { useCoursesStore } from '@/js/stores/courses'
import { categories, levels } from '@/mock/courses'

const router = useRouter()
const authStore = useAuthStore()
const coursesStore = useCoursesStore()

// Локальные ссылки на данные из store
const loading = computed(() => coursesStore.loading)
const currentPage = computed(() => coursesStore.currentPage)
const itemsPerPage = computed(() => coursesStore.itemsPerPage)
const sortBy = computed({
  get: () => coursesStore.sortBy,
  set: (value) => coursesStore.setSortBy(value)
})

// Локальное состояние для фильтров (двустороннее связывание)
const filters = ref({ ...coursesStore.filters })

// Вычисляемые свойства
const filteredCourses = computed(() => coursesStore.filteredAndSortedCourses)
const totalPages = computed(() => Math.ceil(filteredCourses.value.length / itemsPerPage.value))
const paginatedCourses = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value
  const end = start + itemsPerPage.value
  return filteredCourses.value.slice(start, end)
})

const hasActiveFilters = computed(() => {
  return filters.value.search ||
         filters.value.category ||
         filters.value.level ||
         filters.value.priceRange !== 'all' ||
         filters.value.duration !== 'all'
})

// Пагинация: какие страницы показывать
const visiblePages = computed(() => {
  const pages = []
  const maxVisible = 5
  let start = Math.max(1, currentPage.value - Math.floor(maxVisible / 2))
  let end = Math.min(totalPages.value, start + maxVisible - 1)

  if (end - start + 1 < maxVisible) {
    start = Math.max(1, end - maxVisible + 1)
  }

  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  return pages
})

// Аутентификация
const isAuthenticated = computed(() => authStore.isAuthenticated)
const userName = computed(() => authStore.userName)

// Методы
const applyFilters = () => {
  coursesStore.setFilter('search', filters.value.search)
  coursesStore.setFilter('category', filters.value.category)
  coursesStore.setFilter('level', filters.value.level)
  coursesStore.setFilter('priceRange', filters.value.priceRange)
  coursesStore.setFilter('duration', filters.value.duration)
}
const onSearchChange = () => {
  applyFilters()
}

const onSortChange = () => {
  coursesStore.setSortBy(sortBy.value)
}

const removeFilter = (key) => {
  if (key === 'search') {
    filters.value.search = ''
  } else if (key === 'category') {
    filters.value.category = ''
  } else if (key === 'level') {
    filters.value.level = ''
  } else if (key === 'priceRange') {
    filters.value.priceRange = 'all'
  } else if (key === 'duration') {
    filters.value.duration = 'all'
  }
  applyFilters()
}

const resetAllFilters = () => {
  filters.value = {
    search: '',
    category: '',
    level: '',
    priceRange: 'all',
    duration: 'all'
  }
  applyFilters()
  coursesStore.setSortBy('popular')
}

const changePage = (page) => {
  if (page < 1 || page > totalPages.value) return
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
onMounted(() => {
  coursesStore.fetchCourses()
})

// Следим за изменением фильтров для обновления URL (сохранение состояния)
watch([filters, sortBy], () => {
  // TODO: сохранять фильтры в URL (опционально)
}, { deep: true })
</script>

<style scoped>
/* Скелетоны */
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

/* Уровни сложности */
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
  background: var(--error) !important;
  color: white !important;
  outline: none !important;
  box-shadow: none !important;
  border: none !important;
}

.reset-btn:hover {
  background: #dc2626 !important;
  color: white !important;
}

.reset-btn:focus,
.reset-btn:active {
  outline: none !important;
  box-shadow: none !important;
}

.user-name {
  font-weight: 500;
  color: var(--text-primary);
}

.course-card-content {
    text-align: left !important;
}

.course-card-title {
    text-align: left !important;
    justify-content: flex-start !important;
}

</style>
