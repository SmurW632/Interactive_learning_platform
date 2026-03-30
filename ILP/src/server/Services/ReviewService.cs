using server.Data;
using server.Models.ILP;

namespace server.Services;

public class CreateReviewRequest
{
    public Guid CourseId { get; set; }
    public short Rating { get; set; }
    public string? ReviewText { get; set; }
}

public class ReviewResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid UserId { get; set; }
    public short Rating { get; set; }
    public string? ReviewText { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
    public string AuthorName { get; set; }
}

public interface IReviewService
{
    Task<ReviewResponse> CreateReviewAsync(Guid userId, CreateReviewRequest request);
    Task DeleteReviewAsync(Guid reviewId, Guid userId);
    Task<List<ReviewResponse>> GetCourseReviewsAsync(Guid courseId);
    Task<double> GetAverageRatingAsync(Guid courseId);
}

public class ReviewService : IReviewService
{
    private IUnitOfWork _unitOfWork;
    private ILogger<ReviewService> _logger;

    public ReviewService(IUnitOfWork unitOfWork, ILogger<ReviewService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReviewResponse> CreateReviewAsync(Guid userId, CreateReviewRequest request)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = request.CourseId,
            Rating = request.Rating,
            ReviewText = request.ReviewText,
            IsApproved = false,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation("Создан отзыв: " + review.Id + " от пользователя: " + userId);

        return new ReviewResponse
        {
            Id = review.Id,
            CourseId = review.CourseId,
            UserId = review.UserId,
            Rating = review.Rating,
            ReviewText = review.ReviewText,
            IsApproved = review.IsApproved,
            CreatedAt = review.CreatedAt,
            AuthorName = string.Empty
        };
    }

    public async Task DeleteReviewAsync(Guid reviewId, Guid userId)
    {
        var reviews = await _unitOfWork.Reviews.GetListAsync();
        var review = reviews.FirstOrDefault(r => r.Id == reviewId);

        if (review == null)
            return;

        await _unitOfWork.Reviews.DeleteAsync(review.Id);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<List<ReviewResponse>> GetCourseReviewsAsync(Guid courseId)
    {
        var reviews = await _unitOfWork.Reviews.GetListAsync();

        return reviews
            .Where(r => r.CourseId == courseId)
            .Select(r => new ReviewResponse
            {
                Id = r.Id,
                CourseId = r.CourseId,
                UserId = r.UserId,
                Rating = r.Rating,
                ReviewText = r.ReviewText,
                IsApproved = r.IsApproved,
                CreatedAt = r.CreatedAt,
                AuthorName = string.Empty
            })
            .ToList();
    }

    public async Task<double> GetAverageRatingAsync(Guid courseId)
    {
        var reviews = await _unitOfWork.Reviews.GetListAsync();
        var courseReviews = reviews.Where(r => r.CourseId == courseId).ToList();

        return courseReviews.Average(r => r.Rating);
    }
}
