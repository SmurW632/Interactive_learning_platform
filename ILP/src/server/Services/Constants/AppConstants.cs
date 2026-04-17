namespace server.Constants;

public static class AppConstants
{
    public static class Cors
    {
        public const string AllowVueClient = "AllowVueClient";
        public const string DefaultPolicy = "DefaultCorsPolicy";
    }

    public static class Auth
    {
        public const string AdminOnly = "AdminOnly";
        public const string TeacherOnly = "TeacherOnly";
        public const string StudentOnly = "StudentOnly";
    }

    public static class Swagger
    {
        public const string Title = "Интерактивная платформа для обучения API";
        public const string Version = "v1";
        public const string Endpoint = "/swagger/v1/swagger.json";
        public const string RoutePrefix = "swagger";
    }

    public static class Ports
    {
        public const string Https = "https://localhost:5005";
        public const string Http = "http://localhost:5004";
    }
}
