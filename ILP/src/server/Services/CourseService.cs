// server/Services/CourseService.cs
using server.Data;
using server.Data.DTOs;
using server.Data.Repositories;
using server.Models.ILP;

namespace server.Services;

public interface ICourseService
{
    // Для каталога
    Task<PagedResult<CourseDto>> GetCoursesAsync(CourseFilterDto filter, CancellationToken cancellationToken = default);
    Task<CourseDetailDto?> GetCourseByIdAsync(Guid courseId, Guid? userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    // Для страницы курса
    Task<CourseFullDetailDto?> GetCourseFullDetailAsync(Guid courseId, Guid? userId, CancellationToken cancellationToken = default);
    Task<LessonContentDto?> GetLessonContentAsync(Guid courseId, Guid lessonId, Guid? userId, CancellationToken cancellationToken = default);
    Task<EnrollmentResultDto> StartCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<ProgressUpdateResultDto> UpdateLessonProgressAsync(Guid userId, Guid lessonId, UpdateProgressRequest request, CancellationToken cancellationToken = default);
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
            AuthorId = c.CreatedBy,
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

        var completedLessonIds = new HashSet<Guid>();
        if (enrollment != null)
        {
            var completedLessons = await _unitOfWork.LessonProgresses
                .GetEnrollmentProgressAsync(enrollment.Id, cancellationToken);
            completedLessonIds = [.. completedLessons
                .Where(lp => lp.Status == LessonProgressStatus.Completed)
                .Select(lp => lp.LessonId)];
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
            AuthorId = course.CreatedBy,
            Category = course.Category?.Name,
            IsEnrolled = isEnrolled,
            ProgressPercent = enrollment?.ProgressPercent ?? 0,
            Modules = course.Modules?.Select(m => new ModuleDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                SortOrder = m.SortOrder,
                Lessons = [.. m.Lessons.Select(l => new LessonDto
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
                })]
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

    // ==================== МЕТОДЫ ДЛЯ СТРАНИЦЫ КУРСА ====================

    public async Task<CourseFullDetailDto?> GetCourseFullDetailAsync(Guid courseId, Guid? userId, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(courseId, cancellationToken);

        if (course == null)
            return null;

        var isEnrolled = userId.HasValue &&
            await _unitOfWork.Courses.IsUserEnrolledAsync(userId.Value, courseId, cancellationToken);

        var enrollment = isEnrolled && userId.HasValue
            ? await _unitOfWork.Enrollments.GetUserEnrollmentAsync(userId.Value, courseId, cancellationToken)
            : null;

        // Получаем прогресс уроков для студента
        var completedLessonIds = new HashSet<Guid>();
        var lessonProgressMap = new Dictionary<Guid, LessonProgressDetailDto>();

        if (enrollment != null)
        {
            var progresses = await _unitOfWork.LessonProgresses
                .GetEnrollmentProgressAsync(enrollment.Id, cancellationToken);

            completedLessonIds = [.. progresses
                .Where(lp => lp.Status == LessonProgressStatus.Completed)
                .Select(lp => lp.LessonId)];

            foreach (var progress in progresses)
            {
                lessonProgressMap[progress.LessonId] = new LessonProgressDetailDto
                {
                    Status = progress.Status.ToString(),
                    LastPositionSeconds = progress.LastPositionSeconds,
                    TimeWatchedSeconds = progress.TimeWatchedSeconds,
                    IsCompleted = progress.Status == LessonProgressStatus.Completed,
                    CompletedAt = progress.CompletedAt
                };
            }
        }

        // Подсчет статистики
        var allLessons = course.Modules?.SelectMany(m => m.Lessons ?? []).ToList() ?? [];
        var totalLessons = allLessons.Count;
        var completedLessons = completedLessonIds.Count;
        var totalDuration = allLessons.Sum(l => l.DurationMinutes ?? 0);
        var progressPercent = totalLessons > 0 ? (decimal)completedLessons / totalLessons * 100 : 0;

        return new CourseFullDetailDto
        {
            Id = course.Id,
            Title = course.Title,
            Slug = course.Slug,
            Description = course.Description,
            ShortDescription = course.ShortDescription,
            PreviewImageUrl = course.PreviewImageUrl,
            Level = course.Level.ToString(),
            DurationHours = course.DurationHours,
            TotalDurationMinutes = totalDuration,
            LessonsCount = totalLessons,
            CompletedLessonsCount = completedLessons,
            ProgressPercent = progressPercent,
            AverageRating = course.Reviews.Any() ? course.Reviews.Average(r => r.Rating) : 0,
            TotalReviews = course.Reviews.Count(r => r.IsApproved),
            Price = course.Price,
            IsFree = course.IsFree,
            Author = $"{course.Creator.FirstName} {course.Creator.LastName}",
            AuthorAvatar = course.Creator.AvatarUrl,
            Category = course.Category?.Name,
            IsEnrolled = isEnrolled,
            LastAccessedAt = enrollment?.LastAccessedAt,
            WhatYouWillLearn = GetWhatYouWillLearn(course.Title),
            Modules = course.Modules?.OrderBy(m => m.SortOrder).Select(m => new ModuleFullDetailDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                SortOrder = m.SortOrder,
                TotalLessons = m.Lessons?.Count ?? 0,
                TotalDuration = m.Lessons?.Sum(l => l.DurationMinutes ?? 0) ?? 0,
                Lessons = m.Lessons?.OrderBy(l => l.SortOrder).Select(l => new LessonFullDetailDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    ContentType = l.ContentType.ToString(),
                    DurationMinutes = l.DurationMinutes,
                    SortOrder = l.SortOrder,
                    IsFreePreview = l.IsFreePreview,
                    IsCompleted = completedLessonIds.Contains(l.Id),
                    Progress = lessonProgressMap.GetValueOrDefault(l.Id)
                }).ToList() ?? []
            }).ToList() ?? [],
            Reviews = [.. course.Reviews
                .Where(r => r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserName = $"{r.User.FirstName} {r.User.LastName}",
                    UserAvatar = r.User.AvatarUrl,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    CreatedAt = r.CreatedAt
                })]
        };
    }

    public async Task<LessonContentDto?> GetLessonContentAsync(Guid courseId, Guid lessonId, Guid? userId, CancellationToken cancellationToken = default)
    {
        var lesson = await _unitOfWork.Lessons.GetLessonWithModuleAsync(lessonId, cancellationToken);

        if (lesson == null || lesson.Module?.CourseId != courseId)
            return null;

        // Проверка доступа
        var isEnrolled = userId.HasValue &&
            await _unitOfWork.Courses.IsUserEnrolledAsync(userId.Value, courseId, cancellationToken);

        if (!isEnrolled && !lesson.IsFreePreview)
            throw new UnauthorizedAccessException("Необходимо записаться на курс для доступа к уроку");

        // Получаем прогресс пользователя
        var isCompleted = false;
        var currentPosition = 0;

        if (isEnrolled && userId.HasValue)
        {
            var enrollment = await _unitOfWork.Enrollments
                .GetUserEnrollmentAsync(userId.Value, courseId, cancellationToken);

            if (enrollment != null)
            {
                var progress = await _unitOfWork.LessonProgresses
                    .GetByEnrollmentAndLessonAsync(enrollment.Id, lessonId, cancellationToken);

                if (progress != null)
                {
                    isCompleted = progress.Status == LessonProgressStatus.Completed;
                    currentPosition = progress.LastPositionSeconds;
                }
            }
        }

        var nextLesson = await GetNextLessonAsync(courseId, lesson.ModuleId, lesson.SortOrder, cancellationToken);
        var prevLesson = await GetPrevLessonAsync(courseId, lesson.ModuleId, lesson.SortOrder, cancellationToken);

        return new LessonContentDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            ContentType = lesson.ContentType.ToString(),
            VideoUrl = lesson.VideoUrl,
            ContentJson = lesson.ContentJson,
            DurationMinutes = lesson.DurationMinutes,
            SortOrder = lesson.SortOrder,
            IsFreePreview = lesson.IsFreePreview,
            NextLessonId = nextLesson?.Id,
            NextLessonTitle = nextLesson?.Title,
            PrevLessonId = prevLesson?.Id,
            PrevLessonTitle = prevLesson?.Title,
            ModuleTitle = lesson.Module?.Title,
            ModuleId = lesson.ModuleId,
            IsCompleted = isCompleted,
            CurrentPositionSeconds = currentPosition
        };
    }

    public async Task<EnrollmentResultDto> StartCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await _unitOfWork.Courses.GetAsync(courseId, cancellationToken);
        if (course == null)
            throw new KeyNotFoundException("Курс не найден");

        var existingEnrollment = await _unitOfWork.Enrollments
            .GetUserEnrollmentAsync(userId, courseId, cancellationToken);

        if (existingEnrollment != null)
        {
            return new EnrollmentResultDto
            {
                IsSuccess = true,
                IsAlreadyEnrolled = true,
                EnrollmentId = existingEnrollment.Id,
                Message = "Вы уже записаны на этот курс"
            };
        }

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            Status = EnrollmentStatus.Active,
            ProgressPercent = 0
        };

        await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return new EnrollmentResultDto
        {
            IsSuccess = true,
            IsAlreadyEnrolled = false,
            EnrollmentId = enrollment.Id,
            Message = "Вы успешно записались на курс"
        };
    }

    public async Task<ProgressUpdateResultDto> UpdateLessonProgressAsync(Guid userId, Guid lessonId, UpdateProgressRequest request, CancellationToken cancellationToken = default)
    {
        var lesson = await _unitOfWork.Lessons.GetLessonWithModuleAsync(lessonId, cancellationToken);
        if (lesson == null)
            throw new KeyNotFoundException("Урок не найден");

        var enrollment = await _unitOfWork.Enrollments
            .GetUserEnrollmentAsync(userId, lesson.Module.CourseId, cancellationToken);

        if (enrollment == null)
            throw new UnauthorizedAccessException("Вы не записаны на этот курс");

        var progress = await _unitOfWork.LessonProgresses
            .GetByEnrollmentAndLessonAsync(enrollment.Id, lessonId, cancellationToken);

        if (progress == null)
        {
            progress = new LessonProgress
            {
                EnrollmentId = enrollment.Id,
                LessonId = lessonId,
                Status = request.Status,
                StartedAt = request.Status == LessonProgressStatus.InProgress ? DateTime.UtcNow : null,
                LastPositionSeconds = request.LastPositionSeconds,
                TimeWatchedSeconds = request.TimeWatchedSeconds
            };
            await _unitOfWork.LessonProgresses.AddAsync(progress, cancellationToken);
        }
        else
        {
            progress.Status = request.Status;
            progress.LastPositionSeconds = request.LastPositionSeconds;
            progress.TimeWatchedSeconds = request.TimeWatchedSeconds;

            if (request.Status == LessonProgressStatus.Completed && progress.CompletedAt == null)
            {
                progress.CompletedAt = DateTime.UtcNow;
                progress.IsPassed = true;
            }
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        // Обновляем общий прогресс курса
        await _unitOfWork.Enrollments.UpdateProgressPercentAsync(enrollment.Id, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        // Получаем обновленный прогресс
        var updatedEnrollment = await _unitOfWork.Enrollments.GetAsync(enrollment.Id, cancellationToken);
        var totalLessons = await GetTotalLessonsCountAsync(lesson.Module.CourseId, cancellationToken);
        var completedLessons = await _unitOfWork.LessonProgresses
            .GetCompletedLessonsCountAsync(enrollment.Id, cancellationToken);

        return new ProgressUpdateResultDto
        {
            IsSuccess = true,
            LessonId = lessonId,
            Status = progress.Status.ToString(),
            IsCompleted = progress.Status == LessonProgressStatus.Completed,
            CourseProgressPercent = updatedEnrollment?.ProgressPercent ?? 0,
            CompletedLessonsCount = completedLessons,
            TotalLessonsCount = totalLessons
        };
    }

    // ==================== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ====================

    private List<string> GetWhatYouWillLearn(string courseTitle)
    {
        var baseSkills = new List<string>
        {
            "Писать чистый и поддерживаемый код",
            "Использовать современные инструменты разработки",
            "Работать с базами данных",
            "Создавать веб-приложения",
            "Отлаживать и тестировать код",
            "Работать в команде с Git",
            "Понимать принципы ООП",
            "Решать реальные бизнес-задачи"
        };

        // Специфичные навыки в зависимости от курса
        if (courseTitle.Contains("Python", StringComparison.OrdinalIgnoreCase))
        {
            baseSkills.Insert(0, "Создавать Telegram ботов");
            baseSkills.Insert(1, "Анализировать данные с Pandas");
        }
        else if (courseTitle.Contains("ASP.NET", StringComparison.OrdinalIgnoreCase))
        {
            baseSkills.Insert(0, "Создавать REST API");
            baseSkills.Insert(1, "Работать с Entity Framework Core");
        }
        else if (courseTitle.Contains("Vue", StringComparison.OrdinalIgnoreCase))
        {
            baseSkills.Insert(0, "Создавать реактивные интерфейсы");
            baseSkills.Insert(1, "Работать с Pinia и Vue Router");
        }
        else if (courseTitle.Contains("SQL", StringComparison.OrdinalIgnoreCase))
        {
            baseSkills.Insert(0, "Писать сложные SQL запросы");
            baseSkills.Insert(1, "Оптимизировать производительность БД");
        }
        else if (courseTitle.Contains("Docker", StringComparison.OrdinalIgnoreCase))
        {
            baseSkills.Insert(0, "Контейнеризировать приложения");
            baseSkills.Insert(1, "Настраивать CI/CD пайплайны");
        }

        return [.. baseSkills.Take(8)];
    }

    private async Task<Lesson?> GetNextLessonAsync(Guid courseId, Guid moduleId, int currentOrder, CancellationToken ct)
    {
        var module = await _unitOfWork.Modules.GetModuleWithLessonsAsync(moduleId, ct);
        if (module == null) return null;

        var nextLesson = module.Lessons?.FirstOrDefault(l => l.SortOrder == currentOrder + 1);
        if (nextLesson != null) return nextLesson;

        // Переход к следующему модулю
        var modules = await _unitOfWork.Modules.GetModulesByCourseAsync(courseId, ct);
        var modulesList = modules.ToList();
        var currentModuleIndex = modulesList.FindIndex(m => m.Id == moduleId);

        if (currentModuleIndex >= 0 && currentModuleIndex + 1 < modulesList.Count)
        {
            var nextModule = modulesList[currentModuleIndex + 1];
            return nextModule.Lessons?.FirstOrDefault();
        }

        return null;
    }

    private async Task<Lesson?> GetPrevLessonAsync(Guid courseId, Guid moduleId, int currentOrder, CancellationToken ct)
    {
        var module = await _unitOfWork.Modules.GetModuleWithLessonsAsync(moduleId, ct);
        if (module == null) return null;

        var prevLesson = module.Lessons?.FirstOrDefault(l => l.SortOrder == currentOrder - 1);
        if (prevLesson != null) return prevLesson;

        // Переход к предыдущему модулю
        var modules = await _unitOfWork.Modules.GetModulesByCourseAsync(courseId, ct);
        var modulesList = modules.ToList();
        var currentModuleIndex = modulesList.FindIndex(m => m.Id == moduleId);

        if (currentModuleIndex > 0)
        {
            var prevModule = modulesList[currentModuleIndex - 1];
            return prevModule.Lessons?.LastOrDefault();
        }

        return null;
    }

    private async Task<int> GetTotalLessonsCountAsync(Guid courseId, CancellationToken ct)
    {
        var modules = await _unitOfWork.Modules.GetModulesByCourseAsync(courseId, ct);
        return modules.Sum(m => m.Lessons?.Count ?? 0);
    }


}
