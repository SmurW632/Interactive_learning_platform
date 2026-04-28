using Microsoft.Extensions.DependencyInjection;

namespace server.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowVueClient", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:54114",
                        "https://localhost:54114",
                        "https://localhost:8080",
                        "http://localhost:8080",
                        "https://localhost:5173",  // Vite
                        "http://localhost:5173",
                        "https://localhost:3000",   // Альтернативные порты
                        "http://localhost:3000",
                        "https://localhost:5001",   // Сервер для тестов
                        "http://localhost:5001"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static WebApplication UseCorsPolicy(this WebApplication app)
    {
        app.UseCors("AllowVueClient");
        return app;
    }
}
