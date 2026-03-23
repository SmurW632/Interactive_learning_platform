using System.ComponentModel.DataAnnotations;


namespace server.Models.ILP
{
    public enum EnrollmentStatus
    {
        Active,
        Completed,
        Dropped
    }

    public class Enrollment
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime LastAccessedAt { get; set; } = DateTime.UtcNow;
        public Guid? CurrentModuleId { get; set; }
        public Guid? CurrentLessonId { get; set; }
        public decimal ProgressPercent { get; set; } = 0;

        public User User { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public Module? CurrentModule { get; set; }
        public Lesson? CurrentLesson { get; set; }
        public ICollection<LessonProgress> LessonProgresses { get; set; } = [];
    }
}
