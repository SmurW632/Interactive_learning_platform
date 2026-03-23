using System.ComponentModel.DataAnnotations;

namespace server.Models.ILP
{
    public enum LessonProgressStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

    public class LessonProgress
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public Guid EnrollmentId { get; set; }

        [Required]
        public Guid LessonId { get; set; }

        public LessonProgressStatus Status { get; set; } = LessonProgressStatus.NotStarted;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int LastPositionSeconds { get; set; } = 0;
        public int TimeWatchedSeconds { get; set; } = 0;
        public int Attempts { get; set; } = 1;
        public bool? IsPassed { get; set; }

        public Enrollment Enrollment { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
    }
}
