using System.ComponentModel.DataAnnotations;

namespace server.Models.ILP
{
    public enum CourseLevel
    {
        Beginner,
        Intermediate,
        Advanced
    }

    public class Course
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ShortDescription { get; set; }

        public string? PreviewImageUrl { get; set; }
        public CourseLevel Level { get; set; } = CourseLevel.Beginner;
        public int? DurationHours { get; set; }
        public string Language { get; set; } = "ru";
        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedAt { get; set; }

        public Guid CreatedBy { get; set; }
        public int? CategoryId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User Creator { get; set; } = null!;
        public ICollection<Module> Modules { get; set; } = [];
        public ICollection<Enrollment> Enrollments { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
