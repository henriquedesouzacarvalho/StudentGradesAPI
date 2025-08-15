using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Integration
{
    public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> Factory;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            // Customize the factory to use a unique in-memory DB per test run
            Factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContext registrations
                    var toRemove = services.Where(d =>
                        d.ServiceType == typeof(DbContextOptions<StudentGradesContext>) ||
                        d.ServiceType == typeof(StudentGradesContext)).ToList();
                    foreach (var d in toRemove)
                    {
                        services.Remove(d);
                    }

                    // Register a single shared in-memory database for the app
                    services.AddDbContext<StudentGradesContext>(options =>
                    {
                        options.UseInMemoryDatabase("StudentGradesDB_Test");
                    });

                    // No seeding here; we'll seed using the app's service provider after the host is built.
                });
            });

            Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost")
            });

            // Ensure database is created and seed initial data using the app's service provider
            using var appScope = Factory.Services.CreateScope();
            var db = appScope.ServiceProvider.GetRequiredService<StudentGradesContext>();
            db.Database.EnsureCreated();
            if (!db.Students.Any())
            {
                var john = new Student { Id = 1, Name = "John Doe", Email = "john.doe@example.com", CreatedAt = DateTime.UtcNow };
                var jane = new Student { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", CreatedAt = DateTime.UtcNow };
                db.Students.AddRange(john, jane);
                db.Grades.AddRange(
                    new Grade { Id = 1, Value = 8.5, Subject = "Mathematics", StudentId = 1, CreatedAt = DateTime.UtcNow },
                    new Grade { Id = 2, Value = 9.0, Subject = "Physics", StudentId = 1, CreatedAt = DateTime.UtcNow },
                    new Grade { Id = 3, Value = 7.5, Subject = "Chemistry", StudentId = 2, CreatedAt = DateTime.UtcNow }
                );
                db.SaveChanges();
            }
        }
    }
}
