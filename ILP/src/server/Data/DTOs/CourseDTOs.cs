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
