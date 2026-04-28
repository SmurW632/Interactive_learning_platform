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

        try
        {
            if (context.Set<User>().Any())
            {
                Console.WriteLine("Data already exists, skipping seed");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking existing data (table might not exist): {ex.Message}");
            // Если таблицы нет, продолжаем сидирование - миграции должны были создать таблицы
            // но если их нет, то Seed создаст данные при EnsureCreated
        }

        Console.WriteLine("🌱 Seeding test data...");

        // ==================== 1. ПОЛЬЗОВАТЕЛИ ====================
        var student1Id = Guid.NewGuid();
        var student2Id = Guid.NewGuid();
        var student3Id = Guid.NewGuid();
        var student4Id = Guid.NewGuid();
        var student5Id = Guid.NewGuid();
        var teacher1Id = Guid.NewGuid();
        var teacher2Id = Guid.NewGuid();
        var teacher3Id = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        var users = new List<User>
        {
            new() {
                Id = student1Id,
                Email = "ivan.student@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                FirstName = "Иван",
                LastName = "Студентов",
                AvatarUrl = "https://randomuser.me/api/portraits/men/1.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new() {
                Id = student2Id,
                Email = "anna.student@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                FirstName = "Анна",
                LastName = "Петрова",
                AvatarUrl = "https://randomuser.me/api/portraits/women/1.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-28),
                UpdatedAt = DateTime.UtcNow.AddDays(-28)
            },
            new() {
                Id = student3Id,
                Email = "alex.student@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                FirstName = "Алексей",
                LastName = "Сидоров",
                AvatarUrl = "https://randomuser.me/api/portraits/men/2.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new() {
                Id = student4Id,
                Email = "elena.student@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                FirstName = "Елена",
                LastName = "Козлова",
                AvatarUrl = "https://randomuser.me/api/portraits/women/2.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new() {
                Id = student5Id,
                Email = "michael.student@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123!"),
                FirstName = "Michael",
                LastName = "Brown",
                AvatarUrl = "https://randomuser.me/api/portraits/men/3.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new() {
                Id = teacher1Id,
                Email = "maria.teacher@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"),
                FirstName = "Мария",
                LastName = "Преподавательская",
                AvatarUrl = "https://randomuser.me/api/portraits/women/3.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new() {
                Id = teacher2Id,
                Email = "alexey.teacher@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"),
                FirstName = "Алексей",
                LastName = "Смирнов",
                AvatarUrl = "https://randomuser.me/api/portraits/men/4.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new() {
                Id = teacher3Id,
                Email = "olga.teacher@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher123!"),
                FirstName = "Ольга",
                LastName = "Волкова",
                AvatarUrl = "https://randomuser.me/api/portraits/women/4.jpg",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new() {
                Id = adminId,
                Email = "admin@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FirstName = "Admin",
                LastName = "System",
                IsActive = true,
                IsEmailVerified = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            }
        };

        context.Set<User>().AddRange(users);
        context.SaveChanges();
        Console.WriteLine($"✅ Created {users.Count} users");

        // ==================== 2. КАТЕГОРИИ ====================
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Программирование", Slug = "programming", Description = "Курсы по языкам программирования и разработке", SortOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Веб-разработка", Slug = "web-development", Description = "Frontend, Backend, Fullstack разработка", SortOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 3, Name = "Data Science", Slug = "data-science", Description = "Анализ данных, машинное обучение, AI", SortOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 4, Name = "Мобильная разработка", Slug = "mobile-dev", Description = "iOS, Android, Cross-platform", SortOrder = 4, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 5, Name = "Дизайн", Slug = "design", Description = "UI/UX, графический дизайн, Figma", SortOrder = 5, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 6, Name = "Маркетинг", Slug = "marketing", Description = "Digital маркетинг, SMM, SEO", SortOrder = 6, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 7, Name = "Бизнес", Slug = "business", Description = "Управление, лидерство, предпринимательство", SortOrder = 7, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 8, Name = "DevOps", Slug = "devops", Description = "CI/CD, Docker, Kubernetes, облачные технологии", SortOrder = 8, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        context.Set<Category>().AddRange(categories);
        context.SaveChanges();
        Console.WriteLine($"✅ Created {categories.Count} categories");

        // ==================== 3. КУРСЫ ====================
        var courses = new List<Course>
        {
            // 1. ASP.NET Core (Программирование)
            new() {
                Id = Guid.NewGuid(),
                Title = "ASP.NET Core 10: Полный курс",
                Slug = "aspnet-core-10-complete",
                Description = "Освойте ASP.NET Core 10 с нуля до профессионала. Изучите создание современных веб-приложений, REST API, Entity Framework Core, аутентификацию и авторизацию, развертывание в облаке.",
                ShortDescription = "Самый полный курс по ASP.NET Core 10 с практическими проектами",
                PreviewImageUrl = "https://picsum.photos/id/0/300/200",
                Level = CourseLevel.Beginner,
                DurationHours = 45,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-15),
                CreatedBy = teacher1Id,
                Price = 8900,
                CategoryId = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            // 2. Vue.js (Веб-разработка)
            new() {
                Id = Guid.NewGuid(),
                Title = "Vue.js 3 Мастерство: от новичка до эксперта",
                Slug = "vue3-mastery",
                Description = "Полное руководство по Vue.js 3. Composition API, Pinia, Vue Router, создание реальных проектов, оптимизация производительности, тестирование.",
                ShortDescription = "Освойте Vue.js 3 и создавайте реактивные веб-приложения",
                PreviewImageUrl = "https://picsum.photos/id/1/300/200",
                Level = CourseLevel.Intermediate,
                DurationHours = 38,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-10),
                CreatedBy = teacher1Id,
                Price = 0,
                CategoryId = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            // 3. Python для анализа данных (Data Science)
            new() {
                Id = Guid.NewGuid(),
                Title = "Python для Data Science и анализа данных",
                Slug = "python-data-science",
                Description = "Изучите Python для анализа данных: NumPy, Pandas, Matplotlib, Seaborn, Scikit-learn. Работа с реальными датасетами, визуализация, машинное обучение.",
                ShortDescription = "Полный курс по анализу данных на Python",
                PreviewImageUrl = "https://picsum.photos/id/2/300/200",
                Level = CourseLevel.Beginner,
                DurationHours = 52,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-20),
                CreatedBy = teacher2Id,
                Price = 9900,
                CategoryId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-28),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            // 4. React Native (Мобильная разработка)
            new() {
                Id = Guid.NewGuid(),
                Title = "React Native: создание мобильных приложений",
                Slug = "react-native-mobile",
                Description = "Создавайте нативные мобильные приложения для iOS и Android с помощью React Native. Навигация, анимации, работа с API, публикация в App Store и Google Play.",
                ShortDescription = "Курс по разработке мобильных приложений на React Native",
                PreviewImageUrl = "https://picsum.photos/id/3/300/200",
                Level = CourseLevel.Intermediate,
                DurationHours = 42,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-18),
                CreatedBy = teacher2Id,
                Price = 7900,
                CategoryId = 4,
                CreatedAt = DateTime.UtcNow.AddDays(-22),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            // 5. UI/UX Дизайн (Дизайн)
            new() {
                Id = Guid.NewGuid(),
                Title = "UI/UX Дизайн: от идеи до прототипа",
                Slug = "ui-ux-design",
                Description = "Изучите основы UI/UX дизайна. Figma, создание прототипов, пользовательские исследования, дизайн-системы, подготовка макетов для разработчиков.",
                ShortDescription = "Полное руководство по UI/UX дизайну с нуля",
                PreviewImageUrl = "https://picsum.photos/id/4/300/200",
                Level = CourseLevel.Beginner,
                DurationHours = 35,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-12),
                CreatedBy = teacher3Id,
                Price = 6900,
                CategoryId = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            },
            // 6. Digital маркетинг (Маркетинг)
            new() {
                Id = Guid.NewGuid(),
                Title = "Digital маркетинг 2025: стратегии и инструменты",
                Slug = "digital-marketing",
                Description = "Полный курс по digital маркетингу: SEO, контекстная реклама, SMM, email маркетинг, аналитика, управление репутацией.",
                ShortDescription = "Современные стратегии digital маркетинга",
                PreviewImageUrl = "https://picsum.photos/id/5/300/200",
                Level = CourseLevel.Beginner,
                DurationHours = 40,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-8),
                CreatedBy = teacher3Id,
                Price = 5900,
                CategoryId = 6,
                CreatedAt = DateTime.UtcNow.AddDays(-18),
                UpdatedAt = DateTime.UtcNow.AddDays(-6)
            },
            // 7. Docker и Kubernetes (DevOps)
            new() {
                Id = Guid.NewGuid(),
                Title = "Docker и Kubernetes: DevOps с нуля",
                Slug = "docker-kubernetes-devops",
                Description = "Освойте контейнеризацию и оркестрацию. Docker, Kubernetes, CI/CD пайплайны, развертывание в облаке, мониторинг и логирование.",
                ShortDescription = "Практический курс по Docker и Kubernetes",
                PreviewImageUrl = "https://picsum.photos/id/6/300/200",
                Level = CourseLevel.Advanced,
                DurationHours = 48,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-5),
                CreatedBy = teacher1Id,
                Price = 10900,
                CategoryId = 8,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            // 8. SQL для начинающих (Бесплатный)
            new() {
                Id = Guid.NewGuid(),
                Title = "SQL для начинающих: от SELECT до оптимизации",
                Slug = "sql-for-beginners",
                Description = "Изучите SQL с нуля: SELECT, JOIN, подзапросы, оконные функции, индексы, оптимизация запросов, работа с большими базами данных.",
                ShortDescription = "Бесплатный курс по SQL с практическими задачами",
                PreviewImageUrl = "https://picsum.photos/id/7/300/200",
                Level = CourseLevel.Beginner,
                DurationHours = 28,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-14),
                CreatedBy = teacher2Id,
                Price = 0,
                CategoryId = 3,
                CreatedAt = DateTime.UtcNow.AddDays(-12),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            // 9. Бизнес-аналитика (Бизнес)
            new() {
                Id = Guid.NewGuid(),
                Title = "Бизнес-аналитика для руководителей",
                Slug = "business-analytics",
                Description = "Курс для руководителей и предпринимателей: ключевые метрики, построение отчетов, дашборды, прогнозирование, принятие решений на основе данных.",
                ShortDescription = "Как использовать данные для роста бизнеса",
                PreviewImageUrl = "https://picsum.photos/id/8/300/200",
                Level = CourseLevel.Intermediate,
                DurationHours = 32,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-7),
                CreatedBy = teacher3Id,
                Price = 4900,
                CategoryId = 7,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            // 10. Git и GitHub (Бесплатный)
            new() {
                Id = Guid.NewGuid(),
                Title = "Git и GitHub: система контроля версий",
                Slug = "git-github",
                Description = "Изучите Git с нуля: основные команды, ветвление, слияние, разрешение конфликтов, работа с GitHub, CI/CD интеграция.",
                ShortDescription = "Бесплатный курс по Git и GitHub",
                PreviewImageUrl = "https://picsum.photos/id/9/300/200",
                Level = CourseLevel.Beginner,
                DurationHours = 15,
                Language = "ru",
                IsPublished = true,
                PublishedAt = DateTime.UtcNow.AddDays(-3),
                CreatedBy = teacher1Id,
                Price = 0,
                CategoryId = 8,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        context.Set<Course>().AddRange(courses);
        context.SaveChanges();
        Console.WriteLine($"✅ Created {courses.Count} courses");

        // ==================== 4. МОДУЛИ И УРОКИ ====================
        var allModules = new List<Module>();
        var allLessons = new List<Lesson>();
        var random = new Random();

        foreach (var course in courses)
        {
            int moduleCount = random.Next(3, 6);
            var modules = new List<Module>();

            for (int m = 1; m <= moduleCount; m++)
            {
                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    CourseId = course.Id,
                    Title = $"Модуль {m}: {GetModuleTitle(m, course.Title)}",
                    Description = $"В этом модуле мы изучим ключевые концепции и применим их на практике.",
                    SortOrder = m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                modules.Add(module);
                allModules.Add(module);
            }

            context.Set<Module>().AddRange(modules);
            context.SaveChanges();

            // Создаем уроки для каждого модуля
            foreach (var module in modules)
            {
                int lessonCount = random.Next(4, 8);
                var lessons = new List<Lesson>();

                for (int l = 1; l <= lessonCount; l++)
                {
                    var lessonType = random.Next(0, 3) switch
                    {
                        0 => LessonContentType.Video,
                        1 => LessonContentType.Text,
                        _ => LessonContentType.Presentation
                    };

                    var lesson = new Lesson
                    {
                        Id = Guid.NewGuid(),
                        ModuleId = module.Id,
                        Title = GetLessonTitle(l, lessonType),
                        ContentType = lessonType,
                        VideoUrl = lessonType == LessonContentType.Video
                            ? "https://www.youtube.com/embed/dQw4w9WgXcQ"
                            : null,
                        ContentJson = lessonType == LessonContentType.Text
                            ? GetSampleTextContent(l)
                            : null,
                        DurationMinutes = random.Next(10, 45),
                        SortOrder = l,
                        IsFreePreview = l == 1,
                        IsRequired = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    lessons.Add(lesson);
                    allLessons.Add(lesson);
                }

                context.Set<Lesson>().AddRange(lessons);
                context.SaveChanges();
            }
        }

        Console.WriteLine($"✅ Created {allModules.Count} modules and {allLessons.Count} lessons");

        // ==================== 5. ЗАПИСИ НА КУРСЫ ====================
        var studentIds = new[] { student1Id, student2Id, student3Id, student4Id, student5Id };
        var enrollments = new List<Enrollment>();
        var enrollmentCounter = 1;

        foreach (var studentId in studentIds)
        {
            // Каждый студент записан на 3-5 курсов
            int coursesToEnroll = random.Next(3, Math.Min(6, courses.Count + 1));
            var shuffledCourses = courses.OrderBy(x => random.Next()).Take(coursesToEnroll);

            foreach (var course in shuffledCourses)
            {
                var enrollment = new Enrollment
                {
                    Id = Guid.NewGuid(),
                    UserId = studentId,
                    CourseId = course.Id,
                    Status = EnrollmentStatus.Active,
                    EnrolledAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    ProgressPercent = random.Next(0, 101),
                    LastAccessedAt = DateTime.UtcNow.AddDays(-random.Next(0, 10))
                };
                enrollments.Add(enrollment);
                enrollmentCounter++;
            }
        }

        // Добавляем специальные записи
        enrollments.Add(new Enrollment
        {
            Id = Guid.NewGuid(),
            UserId = student1Id,
            CourseId = courses[0].Id,
            Status = EnrollmentStatus.Completed,
            EnrolledAt = DateTime.UtcNow.AddDays(-40),
            CompletedAt = DateTime.UtcNow.AddDays(-5),
            ProgressPercent = 100,
            LastAccessedAt = DateTime.UtcNow.AddDays(-5)
        });

        context.Set<Enrollment>().AddRange(enrollments);
        context.SaveChanges();
        Console.WriteLine($"✅ Created {enrollments.Count} enrollments");

        // ==================== 6. ПРОГРЕСС УРОКОВ ====================
        var lessonProgresses = new List<LessonProgress>();

        foreach (var enrollment in enrollments)
        {
            var course = courses.First(c => c.Id == enrollment.CourseId);
            var courseLessons = allLessons.Where(l =>
                allModules.Where(m => m.CourseId == course.Id).Select(m => m.Id).Contains(l.ModuleId)
            ).ToList();

            if (!courseLessons.Any()) continue;

            int completedCount = (int)(courseLessons.Count * (enrollment.ProgressPercent / 100m));

            for (int i = 0; i < courseLessons.Count; i++)
            {
                var lesson = courseLessons[i];
                var status = i < completedCount
                    ? LessonProgressStatus.Completed
                    : (i == completedCount ? LessonProgressStatus.InProgress : LessonProgressStatus.NotStarted);

                var progress = new LessonProgress
                {
                    EnrollmentId = enrollment.Id,
                    LessonId = lesson.Id,
                    Status = status,
                    StartedAt = status != LessonProgressStatus.NotStarted ? DateTime.UtcNow.AddDays(-random.Next(1, 15)) : null,
                    CompletedAt = status == LessonProgressStatus.Completed ? DateTime.UtcNow.AddDays(-random.Next(1, 5)) : null,
                    Attempts = status == LessonProgressStatus.Completed ? random.Next(1, 3) : 1,
                    LastPositionSeconds = status == LessonProgressStatus.InProgress ? random.Next(10, 600) : 0,
                    TimeWatchedSeconds = status == LessonProgressStatus.InProgress ? random.Next(60, 1200) : 0,
                    IsPassed = status == LessonProgressStatus.Completed
                };
                lessonProgresses.Add(progress);
            }
        }

        context.Set<LessonProgress>().AddRange(lessonProgresses);
        context.SaveChanges();
        Console.WriteLine($"✅ Created {lessonProgresses.Count} lesson progress entries");

        // ==================== 7. ОТЗЫВЫ ====================
        var reviews = new List<Review>();
        var reviewTexts = new[]
        {
            "Отличный курс! Очень понравилась подача материала. Преподаватель объясняет сложные вещи простым языком.",
            "Хороший курс, но хотелось бы больше практических заданий. В целом доволен.",
            "Лучший курс по этой теме! Рекомендую всем начинающим.",
            "Материал структурирован, все понятно. Спасибо автору!",
            "Курс помог мне сменить профессию. Огромное спасибо!",
            "Неплохо, но некоторые темы можно было бы раскрыть подробнее.",
            "Отличная теория, но мало практики. Жду продолжения!",
            "Преподаватель - профессионал своего дела. Очень вдохновляет!",
            "Курс супер! Всё четко, по делу, без воды.",
            "Материал актуален, примеры из реальной жизни. Спасибо!"
        };

        foreach (var enrollment in enrollments.Where(e => e.ProgressPercent > 50))
        {
            if (random.Next(0, 100) < 70) // 70% шанс оставить отзыв
            {
                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    UserId = enrollment.UserId,
                    CourseId = enrollment.CourseId,
                    Rating = (short)random.Next(4, 6),
                    ReviewText = reviewTexts[random.Next(reviewTexts.Length)],
                    IsApproved = random.Next(0, 100) < 90, // 90% отзывов одобрено
                    CreatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    UpdatedAt = DateTime.UtcNow.AddDays(-random.Next(1, 5))
                };
                reviews.Add(review);
            }
        }

        context.Set<Review>().AddRange(reviews);
        context.SaveChanges();
        Console.WriteLine($"✅ Created {reviews.Count} reviews");

        // ==================== ВЫВОД СТАТИСТИКИ ====================
        Console.WriteLine("\n📊 SEEDING COMPLETED SUCCESSFULLY!");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"👥 Users: {users.Count}");
        Console.WriteLine($"📁 Categories: {categories.Count}");
        Console.WriteLine($"📚 Courses: {courses.Count}");
        Console.WriteLine($"📦 Modules: {allModules.Count}");
        Console.WriteLine($"📖 Lessons: {allLessons.Count}");
        Console.WriteLine($"📝 Enrollments: {enrollments.Count}");
        Console.WriteLine($"✅ Progress entries: {lessonProgresses.Count}");
        Console.WriteLine($"⭐ Reviews: {reviews.Count}");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

        // Вывод тестовых аккаунтов
        Console.WriteLine("\n🔐 TEST ACCOUNTS:");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.WriteLine($"📧 student@test.com / Student123! (Default student)");
        Console.WriteLine($"📧 teacher@test.com / Teacher123! (Default teacher)");
        Console.WriteLine($"📧 admin@test.com / Admin123! (Administrator)");
        Console.WriteLine("\nAdditional students:");
        Console.WriteLine($"📧 ivan.student@test.com / Student123!");
        Console.WriteLine($"📧 anna.student@test.com / Student123!");
        Console.WriteLine($"📧 alex.student@test.com / Student123!");
        Console.WriteLine($"📧 elena.student@test.com / Student123!");
        Console.WriteLine($"📧 michael.student@test.com / Student123!");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }

    // Вспомогательные методы для генерации контента
    private static string GetModuleTitle(int moduleNumber, string courseTitle)
    {
        var titles = new[]
        {
            "Введение и основы",
            "Продвинутые концепции",
            "Практические примеры",
            "Работа с данными",
            "Оптимизация и лучшие практики",
            "Финальный проект"
        };

        return moduleNumber <= titles.Length
            ? titles[moduleNumber - 1]
            : $"Дополнительный модуль {moduleNumber}";
    }

    private static string GetLessonTitle(int lessonNumber, LessonContentType type)
    {
        var videoTitles = new[]
        {
            "Введение в тему",
            "Основные концепции",
            "Практический пример",
            "Разбор сложных моментов",
            "Лучшие практики",
            "Заключение и итоги"
        };

        var textTitles = new[]
        {
            "Теоретические основы",
            "Документация и ресурсы",
            "Пошаговое руководство",
            "Часто задаваемые вопросы",
            "Дополнительные материалы",
            "Контрольные вопросы"
        };

        var presentationTitles = new[]
        {
            "Визуальное представление",
            "Диаграммы и схемы",
            "Кейс-стади",
            "Анализ примеров",
            "Сравнительный анализ",
            "Резюме и выводы"
        };

        var titles = type switch
        {
            LessonContentType.Video => videoTitles,
            LessonContentType.Text => textTitles,
            LessonContentType.Presentation => presentationTitles,
            _ => videoTitles
        };

        var index = (lessonNumber - 1) % titles.Length;
        return $"{lessonNumber}. {titles[index]}";
    }

    private static string GetSampleTextContent(int lessonNumber)
    {
        return $@"{{
            ""content"": ""<h1>Урок {lessonNumber}</h1><p>В этом уроке мы рассмотрим важные концепции и научимся применять их на практике.</p><h2>Ключевые моменты:</h2><ul><li>Понимание основных принципов</li><li>Практическое применение</li><li>Решение типовых задач</li></ul><h2>Задание:</h2><p>Выполните упражнения для закрепления материала.</p><h2>Итоги:</h2><p>После изучения этого урока вы сможете самостоятельно применять полученные знания.</p>""
        }}";
    }
}
