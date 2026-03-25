using Microsoft.EntityFrameworkCore;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetUserEnrollmentAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Enrollment?> GetEnrollmentWithProgressAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
    Task UpdateProgressPercentAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}

public class EnrollmentRepository(PostgresDbContext context) : Repository<Enrollment>(context), IEnrollmentRepository
{
    public async Task<Enrollment?> GetUserEnrollmentAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId, cancellationToken);
    }

    public async Task<IEnumerable<Enrollment>> GetUserEnrollmentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId && e.Status == EnrollmentStatus.Active)
            .Include(e => e.Course)
                .ThenInclude(c => c.Reviews)
            .ToListAsync(cancellationToken);
    }

    public async Task<Enrollment?> GetEnrollmentWithProgressAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.LessonProgresses)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId, cancellationToken);
    }

    public async Task UpdateProgressPercentAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await GetEnrollmentWithProgressAsync(enrollmentId, cancellationToken);
        if (enrollment == null) return;

        var totalLessons = await _context.Set<Lesson>()
            .Include(l => l.Module)
            .CountAsync(l => l.Module.CourseId == enrollment.CourseId, cancellationToken);

        var completedLessons = enrollment.LessonProgresses
            .Count(lp => lp.Status == LessonProgressStatus.Completed);

        enrollment.ProgressPercent = totalLessons > 0
            ? (decimal)completedLessons / totalLessons * 100
            : 0;

        enrollment.LastAccessedAt = DateTime.UtcNow;

        if (completedLessons == totalLessons && totalLessons > 0)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAt = DateTime.UtcNow;
        }

        await UpdateAsync(enrollment, cancellationToken);
    }
}
