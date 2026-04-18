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
        public string RelativeTime => GetRelativeTime(CreatedAt);

        private string GetRelativeTime(DateTime date)
        {
            var diff = DateTime.UtcNow - date;
            if (diff.Days > 365) return $"{diff.Days / 365} г. назад";
            if (diff.Days > 30) return $"{diff.Days / 30} мес. назад";
            if (diff.Days > 0) return $"{diff.Days} дн. назад";
            if (diff.Hours > 0) return $"{diff.Hours} ч. назад";
            if (diff.Minutes > 0) return $"{diff.Minutes} мин. назад";
            return "только что";
        }
    }

    public class CreateReviewRequest
    {
        [Range(1, 5)]
        public short Rating { get; set; }
        public string? ReviewText { get; set; }
    }
}
