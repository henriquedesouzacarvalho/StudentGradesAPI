using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
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
        var response = await _client.GetAsync(new Uri("/api/students", UriKind.Relative));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_Grades_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync(new Uri("/api/grades", UriKind.Relative));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
