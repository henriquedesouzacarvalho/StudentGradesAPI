using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StudentGradesAPI.Extensions;
using Xunit;

namespace StudentGradesAPI.Tests.Extensions;

public class ControllerExtensionsTests
{
    [Fact]
    public void EntityNotFound_WithValidParameters_ShouldReturnNotFoundObjectResult()
    {
        // Arrange
        var controller = new TestController();
        const string entityName = "Student";
        const int id = 1;

        // Act
        var result = controller.EntityNotFound(entityName, id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundObjectResult>();
        result.StatusCode.Should().Be(404);
        
        var value = result.Value;
        value.Should().NotBeNull();
        var message = value!.GetType().GetProperty("message")?.GetValue(value) as string;
        message.Should().Be("Student with ID 1 not found.");
    }

    [Fact]
    public void EntityNotFound_WithNullController_ShouldThrowArgumentNullException()
    {
        // Arrange
        ControllerBase controller = null!;
        const string entityName = "Student";
        const int id = 1;

        // Act & Assert
        var action = () => controller.EntityNotFound(entityName, id);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void EntityNotFound_WithNullEntityName_ShouldThrowArgumentException()
    {
        // Arrange
        var controller = new TestController();
        string entityName = null!;
        const int id = 1;

        // Act & Assert
        var action = () => controller.EntityNotFound(entityName, id);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EntityNotFound_WithEmptyEntityName_ShouldThrowArgumentException()
    {
        // Arrange
        var controller = new TestController();
        const string entityName = "";
        const int id = 1;

        // Act & Assert
        var action = () => controller.EntityNotFound(entityName, id);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void EntityNotFound_WithWhitespaceEntityName_ShouldThrowArgumentException()
    {
        // Arrange
        var controller = new TestController();
        const string entityName = "   ";
        const int id = 1;

        // Act & Assert
        var action = () => controller.EntityNotFound(entityName, id);
        action.Should().Throw<ArgumentException>();
    }

    private sealed class TestController : ControllerBase
    {
    }
}