import axios from '@/js/utils/axios'
import { defineStore } from 'pinia'

export const useCoursesStore = defineStore('courses', {
  state: () => ({
    courses: [],
    categories: [],
    loading: false,
    error: null,
    // Фильтры
    filters: {
      search: '',
      category: '',
      level: '',
      priceRange: 'all',
      duration: 'all'
    },
    sortBy: 'popular',
    currentPage: 1,
    itemsPerPage: 12,
    totalCount: 0,
    totalPages: 0
  }),

  getters: {
    filteredAndSortedCourses: (state) => {
      // Фильтрация и сортировка теперь на сервере
      // Этот геттер просто возвращает courses из store
      return state.courses
    },

    hasActiveFilters: (state) => {
      return state.filters.search ||
             state.filters.category ||
             state.filters.level ||
             state.filters.priceRange !== 'all' ||
             state.filters.duration !== 'all'
    },

    // Получить параметры запроса для API
    queryParams: (state) => {
      const params = {}

      if (state.filters.search) params.search = state.filters.search
      if (state.filters.category) params.category = state.filters.category
      if (state.filters.level) params.level = state.filters.level
      if (state.filters.priceRange !== 'all') params.priceRange = state.filters.priceRange
      if (state.filters.duration !== 'all') params.duration = state.filters.duration

      params.sortBy = state.sortBy
      params.page = state.currentPage
      params.pageSize = state.itemsPerPage

      return params
    }
  },

  actions: {
    // Загрузка курсов с сервера
    async fetchCourses() {
      this.loading = true
      this.error = null

      try {
        const params = this.queryParams
        const response = await axios.get('/Courses', { params })

        const data = response.data
        this.courses = data.items
        this.totalCount = data.totalCount
        this.currentPage = data.page
        this.totalPages = data.totalPages
        this.itemsPerPage = data.pageSize

        return data
      } catch (error) {
        this.error = error.response?.data?.message || 'Ошибка загрузки курсов'
        console.error('Failed to fetch courses:', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    // Загрузка категорий
    async fetchCategories() {
      try {
        const response = await axios.get('/Courses/categories')
        this.categories = response.data
        return response.data
      } catch (error) {
        console.error('Failed to fetch categories:', error)
        this.categories = []
      }
    },

    // Установка фильтра
    setFilter(key, value) {
      this.filters[key] = value
      this.currentPage = 1 // Сброс страницы при изменении фильтра
      this.fetchCourses() // Перезагружаем курсы
    },

    // Установка сортировки
    setSortBy(sortBy) {
      this.sortBy = sortBy
      this.currentPage = 1
      this.fetchCourses()
    },

    // Установка страницы
    setPage(page) {
      if (page < 1 || page > this.totalPages) return
      this.currentPage = page
      this.fetchCourses()
    },

    // Сброс всех фильтров
    resetFilters() {
      this.filters = {
        search: '',
        category: '',
        level: '',
        priceRange: 'all',
        duration: 'all'
      }
      this.sortBy = 'popular'
      this.currentPage = 1
      this.fetchCourses()
    },

    // Получить детальную информацию о курсе
    async fetchCourseById(courseId) {
      try {
        const response = await axios.get(`/Courses/${courseId}`)
        return response.data
      } catch (error) {
        console.error('Failed to fetch course:', error)
        throw error
      }
    },

    // Запись на курс
    async enrollInCourse(courseId) {
      try {
        const response = await axios.post(`/Courses/${courseId}/enroll`)
        return response.data
      } catch (error) {
        console.error('Failed to enroll in course:', error)
        throw error
      }
    }
  }
})
