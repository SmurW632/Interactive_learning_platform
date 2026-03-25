using Microsoft.EntityFrameworkCore;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface ICourseRepository : IRepository<Course>
{
    Task<IEnumerable<Course>> GetPublishedCoursesAsync(CancellationToken cancellationToken = default);
    Task<Course?> GetCourseWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> IsUserEnrolledAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(Guid courseId, CancellationToken cancellationToken = default);
}

public class CourseRepository(PostgresDbContext context) : Repository<Course>(context), ICourseRepository
{
    public async Task<IEnumerable<Course>> GetPublishedCoursesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsPublished)
            .Include(c => c.Reviews)
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
}
