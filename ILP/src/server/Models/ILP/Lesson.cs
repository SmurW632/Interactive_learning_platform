using System.ComponentModel.DataAnnotations;

namespace server.Models.ILP
{
    public enum LessonContentType
    {
        Video,
        Text,
        Presentation,
        Quiz
    }

    public class Lesson
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid ModuleId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public LessonContentType ContentType { get; set; }

        public string? ContentJson { get; set; } // JSON для текстового контента
        public string? VideoUrl { get; set; }
        public int? DurationMinutes { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public bool IsFreePreview { get; set; } = false;
        public bool IsRequired { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Module Module { get; set; } = null!;
        public ICollection<LessonProgress> LessonProgresses { get; set; } = [];
    }
}
