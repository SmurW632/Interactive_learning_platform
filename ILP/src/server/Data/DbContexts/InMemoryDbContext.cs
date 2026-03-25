using Microsoft.EntityFrameworkCore;
using server.Data.DbContexts;
using server.Models.ILP;

namespace server.Data.DbContexts;

public class InMemoryDbContext : BaseDbContext
{
    public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options) : base(options)
    {
    }
}
