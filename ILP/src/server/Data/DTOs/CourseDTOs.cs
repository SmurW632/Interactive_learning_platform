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
}
