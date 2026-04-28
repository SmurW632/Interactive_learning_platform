using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Data.DbContexts;
using server.Models.ILP;

namespace server.Extensions;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var context = services.GetRequiredService<BaseDbContext>();

        try
        {
            var usePostgres = configuration.GetValue<bool>("Database:UsePostgres");

            if (usePostgres && context is PostgresDbContext postgresContext)
            {
                // Для PostgreSQL применяем миграции
                await postgresContext.Database.EnsureCreatedAsync();
                Console.WriteLine("✅ PostgreSQL database migrated");
            }
            else
            {
                // Для InMemory и SQLite просто создаем
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("✅ Database created");
            }

            // Заполняем тестовыми данными (только если нет данных)
            if (!await context.Set<User>().AnyAsync())
            {
                TestDataSeeder.Seed(context);
                Console.WriteLine("✅ Test data seeded");
            }
            else
            {
                Console.WriteLine("ℹ️ Database already contains data, skipping seed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
            throw;
        }
    }
}
