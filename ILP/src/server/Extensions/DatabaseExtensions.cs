using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Data.DbContexts;

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
                await postgresContext.Database.MigrateAsync();
                Console.WriteLine("✅ PostgreSQL database migrated");
            }
            else
            {
                // Для InMemory и SQLite просто создаем
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("✅ Database created");
            }

            // Заполняем тестовыми данными
            TestDataSeeder.Seed(context);
            Console.WriteLine("✅ Test data seeded");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
            throw;
        }
    }
}
