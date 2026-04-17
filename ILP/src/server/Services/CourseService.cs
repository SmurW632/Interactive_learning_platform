// server/Services/CourseService.cs
using server.Data;
using server.Data.DTOs;
using server.Data.Repositories;
using server.Models.ILP;

namespace server.Services;

public interface ICourseService
{
    Task<PagedResult<CourseDto>> GetCoursesAsync(CourseFilterDto filter, CancellationToken cancellationToken = default);
    Task<CourseDetailDto?> GetCourseByIdAsync(Guid courseId, Guid? userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
}

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CourseDto>> GetCoursesAsync(CourseFilterDto filter, CancellationToken cancellationToken = default)
    {
        var pagedResult = await _unitOfWork.Courses.GetFilteredCoursesAsync(filter, cancellationToken);

        var items = pagedResult.Items.Select(c => new CourseDto
        {
            Id = c.Id,
            Title = c.Title,
            Slug = c.Slug,
            ShortDescription = c.ShortDescription,
            PreviewImageUrl = c.PreviewImageUrl,
            Level = c.Level.ToString(),
            DurationHours = c.DurationHours,
            AverageRating = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0,
            TotalReviews = c.Reviews.Count(r => r.IsApproved),
            LessonsCount = c.Modules?.Sum(m => m.Lessons?.Count ?? 0) ?? 0,
            Price = c.Price,
            IsFree = c.IsFree,
            Author = $"{c.Creator.FirstName} {c.Creator.LastName}",
            Category = c.Category?.Name
        }).ToList();

        return new PagedResult<CourseDto>
        {
            Items = items,
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<CourseDetailDto?> GetCourseByIdAsync(Guid courseId, Guid? userId, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(courseId, cancellationToken);

        if (course == null)
            return null;

        var isEnrolled = userId.HasValue &&
            await _unitOfWork.Courses.IsUserEnrolledAsync(userId.Value, courseId, cancellationToken);

        var enrollment = isEnrolled && userId.HasValue
            ? await _unitOfWork.Enrollments.GetUserEnrollmentAsync(userId.Value, courseId, cancellationToken)
            : null;

        // Получаем завершенные уроки для пользователя
        var completedLessonIds = new HashSet<Guid>();
        if (enrollment != null)
        {
            var completedLessons = await _unitOfWork.LessonProgresses
                .GetEnrollmentProgressAsync(enrollment.Id, cancellationToken);
            completedLessonIds = completedLessons
                .Where(lp => lp.Status == LessonProgressStatus.Completed)
                .Select(lp => lp.LessonId)
                .ToHashSet();
        }

        return new CourseDetailDto
        {
            Id = course.Id,
            Title = course.Title,
            Slug = course.Slug,
            Description = course.Description,
            ShortDescription = course.ShortDescription,
            PreviewImageUrl = course.PreviewImageUrl,
            Level = course.Level.ToString(),
            DurationHours = course.DurationHours,
            AverageRating = course.Reviews.Any() ? course.Reviews.Average(r => r.Rating) : 0,
            TotalReviews = course.Reviews.Count(r => r.IsApproved),
            LessonsCount = course.Modules?.Sum(m => m.Lessons?.Count ?? 0) ?? 0,
            Price = course.Price,
            IsFree = course.IsFree,
            Author = $"{course.Creator.FirstName} {course.Creator.LastName}",
            Category = course.Category?.Name,
            IsEnrolled = isEnrolled,
            ProgressPercent = enrollment?.ProgressPercent ?? 0,
            Modules = course.Modules?.Select(m => new ModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                SortOrder = m.SortOrder,
                Lessons = m.Lessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    ContentType = l.ContentType.ToString(),
                    VideoUrl = l.VideoUrl,
                    DurationMinutes = l.DurationMinutes,
                    SortOrder = l.SortOrder,
                    IsFreePreview = l.IsFreePreview,
                    IsCompleted = completedLessonIds.Contains(l.Id),
                    ProgressStatus = completedLessonIds.Contains(l.Id)
                        ? LessonProgressStatus.Completed
                        : null
                }).ToList()
            }).ToList() ?? []
        };
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _unitOfWork.Courses.GetAllCategoriesAsync(cancellationToken);

        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Slug = c.Slug,
            Description = c.Description,
            ParentId = c.ParentId,
            SortOrder = c.SortOrder
        });
    }
}
