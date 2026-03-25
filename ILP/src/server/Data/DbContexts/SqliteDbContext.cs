// server/Data/SqliteDbContext.cs
using Microsoft.EntityFrameworkCore;
using server.Data.DbContexts;
using server.Models.ILP;

namespace server.Data.DbContexts;

public class SqliteDbContext : BaseDbContext
{
    public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Тестовые данные
    }
}
