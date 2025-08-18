using Microsoft.EntityFrameworkCore;
using StudentGradesAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework with In-Memory Database
builder.Services.AddDbContext<StudentGradesContext>(options =>
    options.UseInMemoryDatabase("StudentGradesDB"));

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        _ = builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

// Add CORS support
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddDefaultPolicy(corsBuilder =>
        {
            corsBuilder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
        });
    }
    else
    {
        options.AddDefaultPolicy(corsBuilder =>
        {
            corsBuilder.WithOrigins("https://localhost:3000", "https://my-domain.com")
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .WithHeaders("Content-Type", "Authorization", "Accept")
                    .AllowCredentials();
        });
    }
});

app.UseAuthorization();

app.MapControllers();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StudentGradesContext>();
    _ = context.Database.EnsureCreatedAsync();
}

await app.RunAsync();

// Make the implicit Program class public for testing
internal static partial class Program
{
}
