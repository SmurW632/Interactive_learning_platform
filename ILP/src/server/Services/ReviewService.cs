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
    public string AuthorName { get; set; } = string.Empty;
}

public interface IReviewService
{
    Task<ReviewResponse> CreateReviewAsync(Guid userId, CreateReviewRequest request, CancellationToken ct = default);
    Task DeleteReviewAsync(Guid reviewId, Guid userId, CancellationToken ct = default);
    Task<List<ReviewResponse>> GetCourseReviewsAsync(Guid courseId, CancellationToken ct = default);
    Task<double> GetAverageRatingAsync(Guid courseId, CancellationToken ct = default);
}

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(IUnitOfWork unitOfWork, ILogger<ReviewService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ReviewResponse> CreateReviewAsync(
        Guid userId, CreateReviewRequest request, CancellationToken ct = default)
    {
        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = request.CourseId,
            Rating = request.Rating,
            ReviewText = request.ReviewText,
            IsApproved = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.CompleteAsync(ct);

        _logger.LogInformation(
            "Создан отзыв {ReviewId} от пользователя {UserId}", review.Id, userId);

        return MapToResponse(review);
    }

    public async Task DeleteReviewAsync(
        Guid reviewId, Guid userId, CancellationToken ct = default)
    {
        var reviews = await _unitOfWork.Reviews.GetListAsync(ct);
        var review = reviews.FirstOrDefault();

        if (review == null)
            throw new KeyNotFoundException($"Отзыв {reviewId} не найден.");

        if (review.UserId != userId)
            throw new UnauthorizedAccessException("Нет прав на удаление этого отзыва.");

        await _unitOfWork.Reviews.DeleteAsync(review.Id);
        await _unitOfWork.CompleteAsync(ct);
    }

    public async Task<List<ReviewResponse>> GetCourseReviewsAsync(
        Guid courseId, CancellationToken ct = default)
    {
        var reviews = await _unitOfWork.Reviews
            .GetListAsync(ct);

        return reviews.Select(MapToResponse).ToList();
    }

    public async Task<double> GetAverageRatingAsync(
        Guid courseId, CancellationToken ct = default)
    {
        var reviews = await _unitOfWork.Reviews
            .GetListAsync(ct);

        return reviews.Count() == 0 ? 0.0 : reviews.Average(r => r.Rating);
    }

    private static ReviewResponse MapToResponse(Review r) => new()
    {
        Id = r.Id,
        CourseId = r.CourseId,
        UserId = r.UserId,
        Rating = r.Rating,
        ReviewText = r.ReviewText,
        IsApproved = r.IsApproved,
        CreatedAt = r.CreatedAt,
        AuthorName = r.User?.FirstName ?? string.Empty
    };
}
