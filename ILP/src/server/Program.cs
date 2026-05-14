using server.Data;
using server.Extensions;
using static server.Constants.AppConstants;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка хоста
var port = Environment.GetEnvironmentVariable("PORT") ?? "5004";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// 2. Добавление базовых сервисов
builder.Services.AddControllersAndApiExplorer();

// 3. Настройка Swagger документации
builder.Services.AddSwaggerDocumentation();

// 4. Настройка базы данных
builder.Services.AddDatabase(builder.Configuration);

// 5. Регистрация сервисов приложения
builder.Services.AddApplicationServices();

// 6. Настройка JWT аутентификации
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

// 7. Настройка CORS
builder.Services.AddCorsPolicy();

var app = builder.Build();

// 8. Инициализация базы данных
await app.InitializeDatabaseAsync();

// 9. Настройка HTTP pipeline
app.ConfigurePipeline();

// 10. Запуск приложения
app.Run();
