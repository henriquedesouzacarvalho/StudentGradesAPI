using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using StudentGradesAPI.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace StudentGradesAPI.Tests.Integration;

public class WebApplicationFactoryTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WebApplicationFactoryTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetStudents_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/students");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGrades_ShouldReturnSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/grades");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostStudent_ShouldCreateStudent()
    {
        // Arrange
        var createDto = new CreateStudentDto
        {
            Name = "Integration Test Student",
            Email = "integration@test.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/students", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var student = JsonSerializer.Deserialize<StudentResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        student.Should().NotBeNull();
        student!.Name.Should().Be("Integration Test Student");
        student.Email.Should().Be("integration@test.com");
    }

    [Fact]
    public async Task GetStudent_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/students/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetGrade_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/grades/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void Application_ShouldHaveCorrectServices()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var services = scope.ServiceProvider;

        // Act & Assert
        var context = services.GetService<StudentGradesContext>();
        context.Should().NotBeNull();
    }

    [Fact]
    public async Task SwaggerEndpoint_ShouldBeAccessible()
    {
        // Act
        var response = await _client.GetAsync("/swagger/index.html");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HealthCheck_RootEndpoint_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}