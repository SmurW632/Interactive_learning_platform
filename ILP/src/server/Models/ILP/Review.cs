using System.ComponentModel.DataAnnotations;

namespace server.Models.ILP
{
    public class Review
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid CourseId { get; set; }

        [Required]
        [Range(1, 5)]
        public short Rating { get; set; }

        public string? ReviewText { get; set; }
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
