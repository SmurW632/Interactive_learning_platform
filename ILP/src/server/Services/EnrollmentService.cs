using server.Data;
using server.Models.ILP;

namespace server.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponse> EnrollAsync(Guid userId, Guid courseId, CancellationToken ct = default);
    Task<bool> UnenrollAsync(Guid userId, Guid courseId, CancellationToken ct = default);
    Task<List<EnrollmentResponse>> GetUserEnrollmentsAsync(Guid userId, CancellationToken ct = default);
}

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EnrollmentResponse> EnrollAsync(Guid userId, Guid courseId, CancellationToken ct = default)
    {
        var course = await _unitOfWork.Courses.GetAsync(courseId, ct)
            ?? throw new KeyNotFoundException($"Курс с Id {courseId} не найден.");

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            Status = EnrollmentStatus.Active
        };

        await _unitOfWork.Enrollments.AddAsync(enrollment, ct);
        await _unitOfWork.CompleteAsync(ct);

        return new EnrollmentResponse
        {
            Id = enrollment.Id,
            CourseId = courseId,
            CourseTitle = course.Title,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status.ToString()
        };
    }

    public async Task<bool> UnenrollAsync(Guid userId, Guid courseId, CancellationToken ct = default)
    {
        var enrollment = await _unitOfWork.Enrollments
            .GetUserEnrollmentAsync(userId, courseId, ct);

        if (enrollment == null)
            return false;

        await _unitOfWork.Enrollments.DeleteAsync(enrollment.Id, ct);
        await _unitOfWork.CompleteAsync(ct);

        return true;
    }

    public async Task<List<EnrollmentResponse>> GetUserEnrollmentsAsync(Guid userId, CancellationToken ct = default)
    {
        var enrollments = await _unitOfWork.Enrollments
            .GetUserEnrollmentsAsync(userId, ct);

        return [.. enrollments
            .Select(e => new EnrollmentResponse
            {
                Id = e.Id,
                CourseId = e.CourseId,
                CourseTitle = e.Course.Title,
                EnrolledAt = e.EnrolledAt,
                Status = e.Status.ToString()
            })];
    }
}

public class EnrollmentResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
}
