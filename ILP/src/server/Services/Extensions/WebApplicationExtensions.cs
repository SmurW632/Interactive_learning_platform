using Microsoft.AspNetCore.Builder;

namespace server.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        // Разработка
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Маршрутизация и перенаправления
        app.UseRouting();

        // CORS
        app.UseCorsPolicy();

        // Swagger
        app.UseSwaggerDocumentation();

        // Аутентификация и авторизация
        app.UseAuthentication();
        app.UseAuthorization();

        // HTTPS (раскомментировать для production)
        // app.UseHttpsRedirection();

        // Контроллеры
        app.MapControllers();

        // Перенаправление корневого пути на Swagger
        app.MapGet("/", context =>
        {
            context.Response.Redirect("/swagger");
            return Task.CompletedTask;
        });

        return app;
    }
}
