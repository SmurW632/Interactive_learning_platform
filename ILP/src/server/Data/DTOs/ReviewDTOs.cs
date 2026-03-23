using System.ComponentModel.DataAnnotations;

namespace server.Data.DTOs
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserAvatar { get; set; }
        public short Rating { get; set; }
        public string? ReviewText { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateReviewRequest
    {
        [Range(1, 5)]
        public short Rating { get; set; }
        public string? ReviewText { get; set; }
    }
}
