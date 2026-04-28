using Microsoft.EntityFrameworkCore;
using server.Data.DbContexts;
using server.Data.DTOs;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> GetPublishedCoursesAsync(CancellationToken cancellationToken = default);
    Task<Course?> GetCourseWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsUserEnrolledAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<PagedResult<Course>> GetFilteredCoursesAsync(CourseFilterDto filter, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);
}

public class CourseRepository(BaseDbContext context) : Repository<Course>(context), ICourseRepository
{
    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsPublished)
            .Include(c => c.Reviews)
            .Include(c => c.Creator)
            .Include(c => c.Category)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Course?> GetCourseWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.Id == id && c.IsPublished)
            .Include(c => c.Modules.OrderBy(m => m.SortOrder))
                .ThenInclude(m => m.Lessons.OrderBy(l => l.SortOrder))
            .Include(c => c.Reviews)
                .ThenInclude(r => r.User)
            .Include(c => c.Creator)
            .Include(c => c.Category)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsUserEnrolledAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Enrollment>()
            .AnyAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);
    }

    public async Task<double> GetAverageRatingAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Review>()
            .Where(r => r.CourseId == courseId && r.IsApproved)
            .AverageAsync(r => (double?)r.Rating, cancellationToken) ?? 0;
    }

    public async Task<PagedResult<Course>> GetFilteredCoursesAsync(CourseFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(c => c.IsPublished)
            .Include(c => c.Reviews)
            .Include(c => c.Creator)
            .Include(c => c.Category)
            .Include(c => c.Modules)
                .ThenInclude(m => m.Lessons)
            .AsQueryable();

        // Поиск
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(c =>
                c.Title.Contains(filter.Search) ||
                (c.ShortDescription != null && c.ShortDescription.Contains(filter.Search)) ||
                c.Creator.FirstName.Contains(filter.Search) ||
                c.Creator.LastName.Contains(filter.Search));
        }

        // Категория
        if (!string.IsNullOrWhiteSpace(filter.Category))
        {
            query = query.Where(c => c.Category != null && c.Category.Name == filter.Category);
        }

        // Уровень
        if (!string.IsNullOrWhiteSpace(filter.Level))
        {
            if (Enum.TryParse<CourseLevel>(filter.Level, out var level))
            {
                query = query.Where(c => c.Level == level);
            }
        }

        // Цена
        if (!string.IsNullOrWhiteSpace(filter.PriceRange))
        {
            switch (filter.PriceRange)
            {
                case "free":
                    query = query.Where(c => c.Price == 0);
                    break;
                case "paid":
                    query = query.Where(c => c.Price > 0);
                    break;
            }
        }

        // Длительность
        if (!string.IsNullOrWhiteSpace(filter.Duration))
        {
            query = filter.Duration switch
            {
                "0-5" => query.Where(c => c.DurationHours <= 5),
                "5-10" => query.Where(c => c.DurationHours > 5 && c.DurationHours <= 10),
                "10-20" => query.Where(c => c.DurationHours > 10 && c.DurationHours <= 20),
                "20" => query.Where(c => c.DurationHours > 20),
                _ => query
            };
        }

        // Сортировка
        query = filter.SortBy switch
        {
            "newest" => query.OrderByDescending(c => c.CreatedAt),
            "price_asc" => query.OrderBy(c => c.Price),
            "price_desc" => query.OrderByDescending(c => c.Price),
            "rating" => query.OrderByDescending(c => c.Reviews.Average(r => (double?)r.Rating) ?? 0),
            "popular" => query.OrderByDescending(c => c.Enrollments.Count),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Course>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Category>()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(cancellationToken);
    }
}
