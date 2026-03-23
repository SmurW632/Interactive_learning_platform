using server.Models.ILP;

namespace server.Data.DTOs
{
    public class UpdateProgressRequest
    {
        public LessonProgressStatus Status { get; set; }
        public int LastPositionSeconds { get; set; }
        public int TimeWatchedSeconds { get; set; }
    }

    public class ProgressDto
    {
        public Guid LessonId { get; set; }
        public LessonProgressStatus Status { get; set; }
        public int LastPositionSeconds { get; set; }
        public int TimeWatchedSeconds { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class CourseProgressDto
    {
        public Guid CourseId { get; set; }
        public decimal ProgressPercent { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public List<LessonProgressDto>? LessonsProgress { get; set; }
    }

    public class LessonProgressDto
    {
        public Guid LessonId { get; set; }
        public string? LessonTitle { get; set; }
        public LessonProgressStatus Status { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
