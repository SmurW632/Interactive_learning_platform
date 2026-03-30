using server.Data;
using server.Data.DTOs;
using server.Models.ILP;

namespace server.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponse> EnrollAsync(Guid userId, Guid courseId);
    Task<bool> UnenrollAsync(Guid userId, Guid courseId);
    Task<List<EnrollmentResponse>> GetUserEnrollmentsAsync(Guid userId);
}

public class EnrollmentService : IEnrollmentService
{
    private IUnitOfWork _unitOfWork;

    public EnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EnrollmentResponse> EnrollAsync(Guid userId, Guid courseId)
    {
        var course = await _unitOfWork.Courses.GetAsync(courseId);

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.Now,
            Status = EnrollmentStatus.Active
        };

        await _unitOfWork.Enrollments.AddAsync(enrollment);
        await _unitOfWork.CompleteAsync();

        return new EnrollmentResponse
        {
            Id = enrollment.Id,
            CourseId = courseId,
            CourseTitle = course.Title,
            EnrolledAt = enrollment.EnrolledAt,
            Status = enrollment.Status.ToString()
        };
    }

    public async Task<bool> UnenrollAsync(Guid userId, Guid courseId)
    {
        var enrollments = await _unitOfWork.Enrollments.GetListAsync();
        var enrollment = enrollments.FirstOrDefault(
            e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment == null)
            return false;

        await _unitOfWork.Enrollments.DeleteAsync(enrollment.Id);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<List<EnrollmentResponse>> GetUserEnrollmentsAsync(Guid userId)
    {
        var enrollments = await _unitOfWork.Enrollments.GetListAsync();

        var result = new List<EnrollmentResponse>();
        foreach (var e in enrollments)
        {
            if (e.UserId == userId)
            {
                var course = await _unitOfWork.Courses.GetAsync(e.CourseId);
                result.Add(new EnrollmentResponse
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    CourseTitle = course.Title,
                    EnrolledAt = e.EnrolledAt,
                    Status = e.Status.ToString()
                });
            }
        }

        return result;
    }
}

public class EnrollmentResponse
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; }
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; }
}
