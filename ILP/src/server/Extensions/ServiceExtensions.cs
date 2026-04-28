using server.Data;
using server.Services;
using server.Services.PythonResearch;

namespace server.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Business Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        // services.AddScoped<IProgressService, ProgressService>();

        // HTTP Clients
        services.AddHttpClient<IPythonResearchService, PythonResearchService>();

        return services;
    }

    public static IServiceCollection AddControllersAndApiExplorer(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        return services;
    }
}
