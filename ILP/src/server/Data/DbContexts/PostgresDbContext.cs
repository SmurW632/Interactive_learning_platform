using Microsoft.EntityFrameworkCore;
using server.Models.ILP;


namespace server.Data.DbContexts
{
    public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : BaseDbContext(options)
    {
    }
}
