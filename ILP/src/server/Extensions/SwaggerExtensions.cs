using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
namespace server.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Интерактивная платформа для обучения API",
                Version = "v1",
                Description = "API для интерактивной онлайн-платформы для обучения",
                Contact = new OpenApiContact
                {
                    Name = "MMAT",
                    Email = "mmat@eduplatform.com"
                }
            });

            // Добавляем поддержку JWT в Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Введите JWT токен. Пример: \"Bearer {token}\"",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement((document) => new OpenApiSecurityRequirement()
            {
                [new OpenApiSecuritySchemeReference("oauth2", document)] = ["readAccess", "writeAccess"]
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerDocumentation(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Platform API v1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "EduPlatform API Documentation";
            c.DefaultModelsExpandDepth(-1); // Скрыть схемы моделей
        });

        return app;
    }
}
