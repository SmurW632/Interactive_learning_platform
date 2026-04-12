import { defineStore } from 'pinia'
import { mockCourses } from '@/mock/courses'

export const useCoursesStore = defineStore('courses', {
  state: () => ({
    courses: [],
    loading: false,
    filters: {
      search: '',
      category: '',
      level: '',
      priceRange: 'all',
      duration: 'all'
    },
    sortBy: 'popular',
    currentPage: 1,
    itemsPerPage: 6
  }),

  getters: {
    filteredAndSortedCourses() {
      let result = [...this.courses]

      // Поиск по названию
      if (this.filters.search) {
        const searchLower = this.filters.search.toLowerCase()
        result = result.filter(course =>
          course.title.toLowerCase().includes(searchLower)
        )
      }

      // Фильтр по категориям
      if (this.filters.category) {
        result = result.filter(course => course.category === this.filters.category)
      }

      // Фильтр по уровню
      if (this.filters.level) {
        result = result.filter(course => course.level === this.filters.level)
      }

      // Фильтр по цене
      if (this.filters.priceRange === 'free') {
        result = result.filter(course => course.isFree)
      } else if (this.filters.priceRange === 'paid') {
        result = result.filter(course => !course.isFree)
      }

      // Фильтр по длительности
      if (this.filters.duration !== 'all') {
        const [min, max] = this.filters.duration.split('-').map(Number)
        result = result.filter(course => {
          if (!course.durationHours) return false
          if (max) {
            return course.durationHours >= min && course.durationHours <= max
          } else {
            return course.durationHours >= min
          }
        })
      }

      // Сортировка
      switch (this.sortBy) {
        case 'price_asc':
          result.sort((a, b) => (a.price || 0) - (b.price || 0))
          break
        case 'price_desc':
          result.sort((a, b) => (b.price || 0) - (a.price || 0))
          break
        case 'rating':
          result.sort((a, b) => b.averageRating - a.averageRating)
          break
        case 'popular':
        default:
          result.sort((a, b) => b.totalReviews - a.totalReviews)
      }

      return result
    },

    paginatedCourses() {
      const start = (this.currentPage - 1) * this.itemsPerPage
      const end = start + this.itemsPerPage
      return this.filteredAndSortedCourses.slice(start, end)
    },

    totalPages() {
      return Math.ceil(this.filteredAndSortedCourses.length / this.itemsPerPage)
    }
  },

  actions: {
    async fetchCourses() {
      this.loading = true
      try {
        await new Promise(resolve => setTimeout(resolve, 500))
        this.courses = mockCourses
      } catch (error) {
        console.error('Error fetching courses:', error)
      } finally {
        this.loading = false
      }
    },

    setFilter(key, value) {
      this.filters[key] = value
      this.currentPage = 1
    },

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
    },

    setPage(page) {
      this.currentPage = page
    },

    setSortBy(sort) {
      this.sortBy = sort
      this.currentPage = 1
    }
  }
})
