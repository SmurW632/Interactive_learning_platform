// server/Data/TestDataSeeder.cs
using Microsoft.EntityFrameworkCore;
using server.Data.DbContexts;
using server.Models.ILP;

namespace server.Data;

public static class TestDataSeeder
{
    public static void Seed(BaseDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Проверяем, есть ли уже данные
        if (context.Set<User>().Any())
            return;

        Console.WriteLine("Seeding test data...");

        // 1. Создаем пользователей с правильными GUID
        var studentId = Guid.NewGuid();
        var teacherId = Guid.NewGuid();
        var johnId = Guid.NewGuid();

        var users = new List<User>
        {
            new User
            {
                Id = studentId,
                Email = "student@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FirstName = "Иван",
                LastName = "Студентов",
                AvatarUrl = "https://randomuser.me/api/portraits/men/1.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Id = teacherId,
                Email = "teacher@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FirstName = "Мария",
                LastName = "Преподавательская",
                AvatarUrl = "https://randomuser.me/api/portraits/women/1.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Id = johnId,
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            }
        };

        context.Set<User>().AddRange(users);
        context.SaveChanges();

        // 2. Создаем курсы
        var aspNetCourseId = Guid.NewGuid();
        var vueCourseId = Guid.NewGuid();

        var courses = new List<Course>
        {
            new Course
            {
                Id = aspNetCourseId,
                Title = "ASP.NET Core 10 с нуля",
                Slug = "aspnet-core-10-from-zero",
                Description = "Полный курс по ASP.NET Core 10. Изучите создание современных веб-приложений с нуля до профи.",
                ShortDescription = "Изучите ASP.NET Core 10 и создавайте мощные веб-приложения",
                PreviewImageUrl = "https://picsum.photos/id/0/300/200",
                Level = CourseLevel.Beginner,
                DurationHours = 40,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-15),
                CreatedBy = teacherId,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new Course
            {
                Id = vueCourseId,
                Title = "Vue.js 3: Мастерство разработки",
                Slug = "vue3-mastery",
                Description = "Полное руководство по Vue.js 3. Композиция API, Pinia, роутинг и создание реальных проектов.",
                ShortDescription = "Освойте Vue.js 3 и создавайте реактивные интерфейсы",
                PreviewImageUrl = "https://picsum.photos/id/1/300/200",
                Level = CourseLevel.Intermediate,
                DurationHours = 35,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-10),
                CreatedBy = teacherId,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-15)
            }
        };

        context.Set<Course>().AddRange(courses);
        context.SaveChanges();

        // 3. Создаем модули
        var module1Id = Guid.NewGuid();
        var module2Id = Guid.NewGuid();

        var modules = new List<Module>
        {
            new Module
            {
                Id = module1Id,
                CourseId = aspNetCourseId,
                Title = "Введение в ASP.NET Core",
                Description = "Основы и настройка окружения",
                SortOrder = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Module
            {
                Id = module2Id,
                CourseId = aspNetCourseId,
                Title = "Создание Web API",
                Description = "REST API, контроллеры, маршрутизация",
                SortOrder = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Set<Module>().AddRange(modules);
        context.SaveChanges();

        // 4. Создаем уроки
        var lesson1Id = Guid.NewGuid();
        var lesson2Id = Guid.NewGuid();

        var lessons = new List<Lesson>
        {
            new Lesson
            {
                Id = lesson1Id,
                ModuleId = module1Id,
                Title = "Введение в .NET 10",
                ContentType = LessonContentType.Video,
                VideoUrl = "https://www.youtube.com/embed/dQw4w9WgXcQ",
                DurationMinutes = 15,
                SortOrder = 1,
                IsFreePreview = true,
                IsRequired = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Lesson
            {
                Id = lesson2Id,
                ModuleId = module1Id,
                Title = "Установка и настройка SDK",
                ContentType = LessonContentType.Text,
                ContentJson = "{\"content\":\"<h1>Установка .NET SDK</h1><p>Скачайте и установите .NET 10 SDK с официального сайта...</p>\"}",
                DurationMinutes = 20,
                SortOrder = 2,
                IsFreePreview = true,
                IsRequired = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Set<Lesson>().AddRange(lessons);
        context.SaveChanges();

        // 5. Создаем записи на курсы
        var enrollmentId = Guid.NewGuid();

        var enrollments = new List<Enrollment>
        {
            new Enrollment
            {
                Id = enrollmentId,
                UserId = studentId,
                CourseId = aspNetCourseId,
                Status = EnrollmentStatus.Active,
                EnrolledAt = DateTime.UtcNow.AddDays(-5),
                ProgressPercent = 30,
                LastAccessedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        context.Set<Enrollment>().AddRange(enrollments);
        context.SaveChanges();

        // 6. Создаем прогресс уроков
        var lessonProgresses = new List<LessonProgress>
        {
            new LessonProgress
            {
                EnrollmentId = enrollmentId,
                LessonId = lesson1Id,
                Status = LessonProgressStatus.Completed,
                CompletedAt = DateTime.UtcNow.AddDays(-4),
                IsPassed = true,
                Attempts = 1,
                LastPositionSeconds = 0,
                TimeWatchedSeconds = 0
            },
            new LessonProgress
            {
                EnrollmentId = enrollmentId,
                LessonId = lesson2Id,
                Status = LessonProgressStatus.InProgress,
                StartedAt = DateTime.UtcNow.AddDays(-3),
                Attempts = 1,
                LastPositionSeconds = 300,
                TimeWatchedSeconds = 300
            }
        };

        context.Set<LessonProgress>().AddRange(lessonProgresses);
        context.SaveChanges();

        // 7. Создаем отзывы
        var reviewId = Guid.NewGuid();

        var reviews = new List<Review>
        {
            new Review
            {
                Id = reviewId,
                UserId = studentId,
                CourseId = aspNetCourseId,
                Rating = 5,
                ReviewText = "Отличный курс! Очень подробно и понятно объясняется материал. Рекомендую всем начинающим!",
                IsApproved = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        context.Set<Review>().AddRange(reviews);
        context.SaveChanges();

        Console.WriteLine($"✅ Seeded {users.Count} users, {courses.Count} courses, {modules.Count} modules, {lessons.Count} lessons, {enrollments.Count} enrollments, {lessonProgresses.Count} progress entries, {reviews.Count} reviews");
    }
}
