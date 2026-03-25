using Microsoft.EntityFrameworkCore;
using server.Data.DbContexts;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetApprovedReviewsByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<Review?> GetUserReviewForCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
    Task<bool> HasUserReviewedAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default);
}

public class ReviewRepository(BaseDbContext context) : Repository<Review>(context), IReviewRepository
{
    public async Task<IEnumerable<Review>> GetApprovedReviewsByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.CourseId == courseId && r.IsApproved)
            .Include(r => r.User)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> GetUserReviewForCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.UserId == userId && r.CourseId == courseId, cancellationToken);
    }

    public async Task<bool> HasUserReviewedAsync(Guid userId, Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(r => r.UserId == userId && r.CourseId == courseId, cancellationToken);
    }
}
