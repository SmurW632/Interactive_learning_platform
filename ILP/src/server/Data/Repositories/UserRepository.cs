using Microsoft.EntityFrameworkCore;
using server.Models.ILP;

namespace server.Data.Repositories;

public interface IUserRepository : IRepository<User>
{

}

public class UserRepository : IUserRepository
{

    private bool disposed = false;
    private readonly PostgresDbContext _db;
    private readonly IConfiguration _configuration;

    public UserRepository(PostgresDbContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    public async Task<IEnumerable<User>> GetListAsync()
    {
        return await _db.Users.ToListAsync();
    }

    public async Task<User> GetAsync(Guid id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user != null) return user;

        return new User();
    }

    public async void UpdateAsync(User user)
    {
        var getUser = await GetAsync(user.Id);
        _db.Entry(getUser).State = EntityState.Modified;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await GetAsync(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            return true;
        }
        return false;
    }

    public async void SaveAsync()
    {
        await _db.SaveChangesAsync();
    }

    public virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _db.Dispose();
            }
        }
        this.disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
