using Microsoft.EntityFrameworkCore;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface ILessonRepository : IRepository<Lesson>
{
    Task<IEnumerable<Lesson>> GetLessonsByModuleAsync(Guid moduleId, CancellationToken cancellationToken = default);
    Task<Lesson?> GetLessonWithModuleAsync(Guid lessonId, CancellationToken cancellationToken = default);
}

public class LessonRepository(PostgresDbContext context) : Repository<Lesson>(context), ILessonRepository
{
    public async Task<IEnumerable<Lesson>> GetLessonsByModuleAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.ModuleId == moduleId)
            .OrderBy(l => l.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<Lesson?> GetLessonWithModuleAsync(Guid lessonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(l => l.Module)
            .FirstOrDefaultAsync(l => l.Id == lessonId, cancellationToken);
    }
}
