using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//builder.Services.AddControllersWithViews();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    app.UseHsts();
}

app.UseCors("AllowVueClient");


//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Platform API v1");
    c.RoutePrefix = "swagger"; // будет доступно по /swagger
});

// app.MapStaticAssets();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Test}/{id?}")
//     .WithStaticAssets();

app.Map("", async (context) => context.Response.Redirect("/swagger"));
app.Run();
