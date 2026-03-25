using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using server.Data.DbContexts;

namespace server.Data;

public static class DatabaseFactory
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var useInMemory = configuration.GetValue<bool>("Database:UseInMemory");
        var useSqlite = configuration.GetValue<bool>("Database:UseSqlite");
        var usePostgres = configuration.GetValue<bool>("Database:UsePostgres");

        if (useInMemory)
        {
            // Регистрируем InMemory контекст
            services.AddDbContext<InMemoryDbContext>(options =>
                options.UseInMemoryDatabase("ILP_InMemory_DB"));

            // Регистрируем BaseDbContext как InMemoryDbContext
            services.AddScoped<BaseDbContext>(sp => sp.GetRequiredService<InMemoryDbContext>());
        }
        else if (useSqlite)
        {
            // Регистрируем SQLite контекст
            services.AddDbContext<SqliteDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("SqliteConnection")));

            services.AddScoped<BaseDbContext>(sp => sp.GetRequiredService<SqliteDbContext>());
        }
        else if (usePostgres)
        {
            // Регистрируем PostgreSQL контекст
            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<BaseDbContext>(sp => sp.GetRequiredService<PostgresDbContext>());
        }
        else
        {
            // По умолчанию InMemory
            services.AddDbContext<InMemoryDbContext>(options =>
                options.UseInMemoryDatabase("ILP_InMemory_DB"));

            services.AddScoped<BaseDbContext>(sp => sp.GetRequiredService<InMemoryDbContext>());
        }

        return services;
    }
}
