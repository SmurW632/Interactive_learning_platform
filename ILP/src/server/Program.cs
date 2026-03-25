using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using server.Data;
using server.Data.DbContexts;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Интерактивная платформа для обучения API",
        Version = "v1",
        Description = "API для интерактивной онлайн-платформы для обучения"
    });

    // Добавляем поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Введите JWT токен",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
});


// Database configuration
builder.Services.AddDatabase(builder.Configuration);

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
// builder.Services.AddScoped<IAuthService, AuthService>();
// builder.Services.AddScoped<ICourseService, CourseService>();
// builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
// builder.Services.AddScoped<IProgressService, ProgressService>();

// JWT Authentication

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is missing. Please check appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
               Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueClient", policy =>
    {
        policy.WithOrigins(
                "https://localhost:8080",
                "http://localhost:8080",
                "https://localhost:5173",  // Vite
                "http://localhost:5173",
                "https://localhost:3000",   // Альтернативные порты
                "http://localhost:3000"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Initialize database with test data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BaseDbContext>();

    // Проверяем тип базы данных
    var usePostgres = builder.Configuration.GetValue<bool>("Database:UsePostgres");

    if (usePostgres && context is PostgresDbContext postgresContext)
    {
        // Для PostgreSQL применяем миграции
        await postgresContext.Database.MigrateAsync();
        Console.WriteLine("PostgreSQL database migrated");
    }
    else
    {
        // Для InMemory и SQLite просто создаем
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("Database created");
    }

    // Заполняем тестовыми данными
    TestDataSeeder.Seed(context);
    Console.WriteLine("Test data seeded");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();

}

app.UseCors("AllowVueClient");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Platform API v1");
    c.RoutePrefix = "swagger";
});
//app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Map("", async (context) => context.Response.Redirect("/swagger"));
app.Run();
