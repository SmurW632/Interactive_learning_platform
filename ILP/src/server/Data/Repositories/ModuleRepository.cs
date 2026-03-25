using Microsoft.EntityFrameworkCore;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface IModuleRepository : IRepository<Module>
{
    Task<IEnumerable<Module>> GetModulesByCourseAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task<Module?> GetModuleWithLessonsAsync(Guid moduleId, CancellationToken cancellationToken = default);
}

public class ModuleRepository(PostgresDbContext context) : Repository<Module>(context), IModuleRepository
{
    public async Task<IEnumerable<Module>> GetModulesByCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.SortOrder)
            .Include(m => m.Lessons.OrderBy(l => l.SortOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<Module?> GetModuleWithLessonsAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(m => m.Lessons.OrderBy(l => l.SortOrder))
            .FirstOrDefaultAsync(m => m.Id == moduleId, cancellationToken);
    }
}
