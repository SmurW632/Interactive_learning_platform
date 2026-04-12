export const mockCourses = [
  {
    id: '1',
    title: 'Продвинутый JavaScript: от основ до архитектуры',
    category: 'Программирование',
    shortDescription: 'Полный курс по современному JavaScript: ES6+, асинхронность, промисы, замыкания, паттерны проектирования, архитектура приложений и подготовка к собеседованию.',
    previewImageUrl: null,
    level: 'Advanced',
    durationHours: 18,
    lessonsCount: 24,
    averageRating: 4.9,
    totalReviews: 1200,
    price: 5900,
    isFree: false,
    author: 'Артем Смирнов'
  },
  {
    id: '2',
    title: 'Python для начинающих: с нуля до первого проекта',
    category: 'Программирование',
    shortDescription: 'Самый популярный курс для новичков. Переменные, типы данных, функции, ООП, работа с файлами, базы данных и создание телеграм-бота.',
    previewImageUrl: null,
    level: 'Beginner',
    durationHours: 24,
    lessonsCount: 32,
    averageRating: 4.8,
    totalReviews: 3400,
    price: 0,
    isFree: true,
    author: 'Елена Попова'
  },
  {
    id: '3',
    title: 'UX/UI Дизайн: полное руководство для начинающих',
    category: 'Дизайн',
    shortDescription: 'Figma, создание прототипов, UX-исследования, дизайн-системы, подготовка макетов для разработчиков и портфолио.',
    previewImageUrl: null,
    level: 'Beginner',
    durationHours: 16,
    lessonsCount: 28,
    averageRating: 4.7,
    totalReviews: 892,
    price: 4900,
    isFree: false,
    author: 'Анна Иванова'
  },
  {
    id: '4',
    title: 'Excel для бизнеса: аналитика и автоматизация',
    category: 'Бизнес',
    shortDescription: 'Сводные таблицы, VLOOKUP, Power Query, макросы, дашборды, визуализация данных и автоматизация отчетов.',
    previewImageUrl: null,
    level: 'Intermediate',
    durationHours: 12,
    lessonsCount: 20,
    averageRating: 4.9,
    totalReviews: 2100,
    price: 3900,
    isFree: false,
    author: 'Михаил Петров'
  },
  {
    id: '5',
    title: 'React с нуля: хуки, контекст, Redux Toolkit',
    category: 'Веб-разработка',
    shortDescription: 'Компоненты, хуки, роутинг, управление состоянием, работа с API, тестирование и деплой React-приложений.',
    previewImageUrl: null,
    level: 'Intermediate',
    durationHours: 22,
    lessonsCount: 36,
    averageRating: 4.6,
    totalReviews: 1200,
    price: 0,
    isFree: true,
    author: 'Дмитрий Соколов'
  },
  {
    id: '6',
    title: 'SQL для аналитики данных и продакт-менеджеров',
    category: 'Data Science',
    shortDescription: 'SELECT, JOIN, подзапросы, оконные функции, оптимизация запросов, работа с большими данными и дашборды.',
    previewImageUrl: null,
    level: 'Intermediate',
    durationHours: 10,
    lessonsCount: 18,
    averageRating: 4.9,
    totalReviews: 1800,
    price: 5900,
    isFree: false,
    author: 'Ольга Волкова'
  },
  {
    id: '7',
    title: 'Machine Learning: от линейной регрессии до нейросетей',
    category: 'Программирование',
    shortDescription: 'Python, NumPy, Pandas, Scikit-learn, TensorFlow, обучение моделей, оценка качества и развертывание.',
    previewImageUrl: null,
    level: 'Advanced',
    durationHours: 32,
    lessonsCount: 42,
    averageRating: 4.7,
    totalReviews: 456,
    price: 8900,
    isFree: false,
    author: 'Алексей Смирнов'
  },
  {
    id: '8',
    title: 'Английский для IT-специалистов и программистов',
    category: 'Программирование',
    shortDescription: 'Технический английский, собеседования, написание резюме, чтение документации, переписка и митинги.',
    previewImageUrl: null,
    level: 'Beginner',
    durationHours: 15,
    lessonsCount: 24,
    averageRating: 4.5,
    totalReviews: 2300,
    price: 0,
    isFree: true,
    author: 'Мария Ли'
  }
]

// Опции для фильтров
export const categories = [
  'Программирование',
  'Веб-разработка',
  'Мобильная разработка',
  'Data Science',
  'Дизайн',
  'Маркетинг',
  'Бизнес'
]

export const levels = [
  { value: 'Beginner', label: 'Начальный' },
  { value: 'Intermediate', label: 'Средний' },
  { value: 'Advanced', label: 'Продвинутый' }
]

export const sortOptions = [
  { value: 'popular', label: 'По популярности' },
  { value: 'price_asc', label: 'По цене (возрастание)' },
  { value: 'price_desc', label: 'По цене (убывание)' },
  { value: 'rating', label: 'По рейтингу' },
  { value: 'newest', label: 'По новизне' }
]
