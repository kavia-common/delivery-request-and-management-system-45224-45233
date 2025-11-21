using DeliveryBackend.Middleware;
using DeliveryBackend.Repositories;
using DeliveryBackend.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Ensure model validation returns consistent responses
        options.InvalidModelStateResponseFactory = context =>
        {
            var dto = DeliveryBackend.DTOs.ErrorResponse.FromModelState(context.ModelState);
            return new BadRequestObjectResult(dto);
        };
    });

// OpenAPI/Swagger via NSwag with metadata and tag groups
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Delivery Backend API";
    config.Description = "Ocean Professional - Delivery service API for users, orders, and tracking.";
    config.Version = "1.0.0";
    config.DocumentName = "v1";
    config.PostProcess = document =>
    {
        document.Tags = new[]
        {
            new NSwag.OpenApiTag { Name = "Users", Description = "User registration and login" },
            new NSwag.OpenApiTag { Name = "Orders", Description = "Place and manage delivery orders" },
            new NSwag.OpenApiTag { Name = "Tracking", Description = "Order tracking information" }
        }.ToList();
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Dependency Injection: repositories (in-memory) and services
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<ITrackingRepository, InMemoryTrackingRepository>();

builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<ITrackingService, TrackingService>();

var app = builder.Build();

// Global error handling
app.UseGlobalErrorHandler();

// Use CORS
app.UseCors("AllowAll");

// Configure OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

// Map controllers
app.MapControllers();

// Health check endpoint (root)
app.MapGet("/", () => Results.Json(new { status = "healthy", service = "delivery-backend", theme = new { primary = "#2563EB", secondary = "#F59E0B" } }));

// Ensure the app runs on port 3001 (respects launchSettings for dev). In hosting envs, Kestrel can use ASPNETCORE_URLS.
var port = Environment.GetEnvironmentVariable("PORT") ?? "3001";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();