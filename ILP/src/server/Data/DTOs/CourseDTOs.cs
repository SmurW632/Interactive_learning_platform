using server.Models.ILP;

namespace server.Data.DTOs
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? PreviewImageUrl { get; set; }
        public string? Level { get; set; }
        public int? DurationHours { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Дополнительные поля для каталога
        public int LessonsCount { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public Guid AuthorId { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
    }

    public class CourseDetailDto : CourseDto
    {
        public string? Description { get; set; }
        public List<ModuleDto>? Modules { get; set; }
        public bool IsEnrolled { get; set; }
        public decimal? ProgressPercent { get; set; }
    }

    public class CourseFullDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? PreviewImageUrl { get; set; }
        public string Level { get; set; } = string.Empty;
        public int? DurationHours { get; set; }

        // Статистика
        public int TotalDurationMinutes { get; set; }
        public int LessonsCount { get; set; }
        public int CompletedLessonsCount { get; set; }
        public decimal ProgressPercent { get; set; }

        // Рейтинг
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Цена
        public decimal Price { get; set; }
        public bool IsFree { get; set; }

        // Автор
        public string Author { get; set; } = string.Empty;
        public string? AuthorAvatar { get; set; }
        public string? AuthorBio { get; set; }

        // Категория
        public string? Category { get; set; }

        // Статус для текущего пользователя
        public bool IsEnrolled { get; set; }
        public DateTime? LastAccessedAt { get; set; }

        // Чему научитесь
        public List<string> WhatYouWillLearn { get; set; } = [];

        // Программа курса
        public List<ModuleFullDetailDto> Modules { get; set; } = [];

        // Отзывы
        public List<ReviewDto> Reviews { get; set; } = [];
    }

    /// <summary>
    /// Полная информация о модуле для страницы курса
    /// </summary>
    public class ModuleFullDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public int TotalLessons { get; set; }
        public int TotalDuration { get; set; }
        public List<LessonFullDetailDto> Lessons { get; set; } = [];
    }

    public class ModuleDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public List<LessonDto>? Lessons { get; set; }
    }

    public class LessonDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? ContentType { get; set; }
        public string? VideoUrl { get; set; }
        public int? DurationMinutes { get; set; }
        public int SortOrder { get; set; }
        public bool IsFreePreview { get; set; }
        public bool IsCompleted { get; set; }
        public LessonProgressStatus? ProgressStatus { get; set; }
    }

    /// <summary>
    /// Полная информация об уроке для страницы курса
    /// </summary>
    public class LessonFullDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public int? DurationMinutes { get; set; }
        public int SortOrder { get; set; }
        public bool IsFreePreview { get; set; }
        public bool IsCompleted { get; set; }
        public LessonProgressDetailDto? Progress { get; set; }
    }

    /// <summary>
    /// Детальная информация о прогрессе урока
    /// </summary>
    public class LessonProgressDetailDto
    {
        public string Status { get; set; } = "NotStarted";
        public int LastPositionSeconds { get; set; }
        public int TimeWatchedSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    /// <summary>
    /// Содержимое урока для просмотра
    /// </summary>
    public class LessonContentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public string? ContentJson { get; set; }
        public int? DurationMinutes { get; set; }
        public int SortOrder { get; set; }
        public bool IsFreePreview { get; set; }

        // Навигация между уроками
        public Guid? NextLessonId { get; set; }
        public string? NextLessonTitle { get; set; }
        public Guid? PrevLessonId { get; set; }
        public string? PrevLessonTitle { get; set; }

        // Информация о модуле
        public string? ModuleTitle { get; set; }
        public Guid? ModuleId { get; set; }

        // Прогресс пользователя
        public bool IsCompleted { get; set; }
        public int? CurrentPositionSeconds { get; set; }
    }

    public class CourseFilterDto
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public string? Level { get; set; }
        public string? PriceRange { get; set; } = "all";
        public string? Duration { get; set; } = "all";
        public string? SortBy { get; set; } = "popular";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }

    /// <summary>
    /// Результат записи на курс
    /// </summary>
    public class EnrollmentResultDto
    {
        public bool IsSuccess { get; set; }
        public bool IsAlreadyEnrolled { get; set; }
        public Guid EnrollmentId { get; set; }
        public string? Message { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
