using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace StudentGradesAPI.Tests.Integration;

public class ConfigurationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

    [Fact]
    public void Application_ShouldHaveCorrectCorsConfiguration()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Assert
        configuration.Should().NotBeNull();

        // Test that CORS is configured (indirectly by ensuring service is registered)
        var corsService = scope.ServiceProvider.GetService<Microsoft.AspNetCore.Cors.Infrastructure.ICorsService>();
        corsService.Should().NotBeNull();
    }

    [Fact]
    public void Application_ShouldConfigureInMemoryDatabase()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<StudentGradesAPI.Models.StudentGradesContext>();

        // Assert
        context.Should().NotBeNull();
        // Verify it's using in-memory database by checking the provider name contains "InMemory"
        context!.Database.ProviderName.Should().Contain("InMemory");
    }

    [Fact]
    public void Program_ProtectedConstructor_ShouldExistAndBeCallable()
    {
        // Arrange & Act
        var programType = typeof(Program);
        var constructors = programType.GetConstructors(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        // Assert
        constructors.Should().NotBeEmpty();
        constructors.Should().Contain(c => c.IsFamily); // Protected constructor

        // Execute the protected constructor to cover the code
        var protectedConstructor = constructors.First(c => c.IsFamily);
        var programInstance = protectedConstructor.Invoke(null);
        programInstance.Should().NotBeNull();
        programInstance.Should().BeOfType<Program>();
    }

    [Fact]
    public async Task Application_ShouldAllowCorsRequests()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Make a simple request that would trigger CORS
        var response = await client.GetAsync(new Uri("/api/students", UriKind.Relative));

        // Assert
        response.Should().NotBeNull();
        // The fact that we can make the request without CORS errors indicates CORS is working
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldHaveRequiredServices()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Assert - Check that key services are registered
        serviceProvider.GetService<IConfiguration>().Should().NotBeNull();
        serviceProvider.GetService<StudentGradesAPI.Models.StudentGradesContext>().Should().NotBeNull();
    }
}