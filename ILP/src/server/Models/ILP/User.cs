using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models.ILP
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }
        public string? Phone { get; set; }
        public string? Timezone { get; set; }
        public DateTime? LastActiveAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public ICollection<Enrollment> Enrollments { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
