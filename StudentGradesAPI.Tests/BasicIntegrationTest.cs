using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace StudentGradesAPI.Tests;

public class BasicIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BasicIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Get_Students_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/students");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Grades_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/grades");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}