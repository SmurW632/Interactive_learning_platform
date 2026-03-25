using Microsoft.EntityFrameworkCore;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> AnyEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateLastActiveAsync(Guid userId, CancellationToken cancellationToken = default);
}

public class UserRepository(PostgresDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<bool> AnyEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetWithEnrollmentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Enrollments)
                .ThenInclude(e => e.Course)
            .Include(u => u.Reviews)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task UpdateLastActiveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await GetAsync(userId, cancellationToken);
        if (user != null)
        {
            user.LastActiveAt = DateTime.UtcNow;
            await UpdateAsync(user, cancellationToken);
        }
    }
}
