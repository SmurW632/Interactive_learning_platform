using Microsoft.EntityFrameworkCore;
using server.Data.DbContexts;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface ILessonProgressRepository : IRepository<LessonProgress>
{
    Task<LessonProgress?> GetByEnrollmentAndLessonAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default);
    Task<IEnumerable<LessonProgress>> GetEnrollmentProgressAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task<int> GetCompletedLessonsCountAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}

public class LessonProgressRepository(BaseDbContext context) : Repository<LessonProgress>(context), ILessonProgressRepository
{
    public async Task<LessonProgress?> GetByEnrollmentAndLessonAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(lp => lp.EnrollmentId == enrollmentId && lp.LessonId == lessonId, cancellationToken);
    }

    public async Task<IEnumerable<LessonProgress>> GetEnrollmentProgressAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(lp => lp.EnrollmentId == enrollmentId)
            .Include(lp => lp.Lesson)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCompletedLessonsCountAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(lp => lp.EnrollmentId == enrollmentId && lp.Status == LessonProgressStatus.Completed, cancellationToken);
    }
}
