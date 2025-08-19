using FluentAssertions;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Models;

public class DTOTests
{
    [Fact]
    public void CreateStudentDto_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var dto = new CreateStudentDto();

        // Assert
        dto.Name.Should().Be(string.Empty);
        dto.Email.Should().Be(string.Empty);
    }

    [Fact]
    public void CreateStudentDto_WithProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedName = "John Doe";
        var expectedEmail = "john.doe@example.com";

        // Act
        var dto = new CreateStudentDto
        {
            Name = expectedName,
            Email = expectedEmail,
        };

        // Assert
        dto.Name.Should().Be(expectedName);
        dto.Email.Should().Be(expectedEmail);
    }

    [Fact]
    public void UpdateStudentDto_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var dto = new UpdateStudentDto();

        // Assert
        dto.Name.Should().BeNull();
        dto.Email.Should().BeNull();
    }

    [Fact]
    public void UpdateStudentDto_WithProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedName = "Jane Smith";
        var expectedEmail = "jane.smith@example.com";

        // Act
        var dto = new UpdateStudentDto
        {
            Name = expectedName,
            Email = expectedEmail,
        };

        // Assert
        dto.Name.Should().Be(expectedName);
        dto.Email.Should().Be(expectedEmail);
    }

    [Fact]
    public void CreateGradeDto_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var dto = new CreateGradeDto();

        // Assert
        dto.Value.Should().Be(0.0);
        dto.Subject.Should().Be(string.Empty);
        dto.StudentId.Should().Be(0);
    }

    [Fact]
    public void CreateGradeDto_WithProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedValue = 8.5;
        var expectedSubject = "Mathematics";
        var expectedStudentId = 1;

        // Act
        var dto = new CreateGradeDto
        {
            Value = expectedValue,
            Subject = expectedSubject,
            StudentId = expectedStudentId,
        };

        // Assert
        dto.Value.Should().Be(expectedValue);
        dto.Subject.Should().Be(expectedSubject);
        dto.StudentId.Should().Be(expectedStudentId);
    }

    [Fact]
    public void UpdateGradeDto_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var dto = new UpdateGradeDto();

        // Assert
        dto.Value.Should().BeNull();
        dto.Subject.Should().BeNull();
    }

    [Fact]
    public void UpdateGradeDto_WithProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedValue = 9.0;
        var expectedSubject = "Physics";

        // Act
        var dto = new UpdateGradeDto
        {
            Value = expectedValue,
            Subject = expectedSubject,
        };

        // Assert
        dto.Value.Should().Be(expectedValue);
        dto.Subject.Should().Be(expectedSubject);
    }

    [Fact]
    public void StudentResponseDto_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var dto = new StudentResponseDto();

        // Assert
        dto.Id.Should().Be(0);
        dto.Name.Should().Be(string.Empty);
        dto.Email.Should().Be(string.Empty);
        dto.CreatedAt.Should().Be(default);
        dto.AverageGrade.Should().Be(0.0);
        dto.Grades.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void StudentResponseDto_WithProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedId = 1;
        var expectedName = "John Doe";
        var expectedEmail = "john.doe@example.com";
        var expectedCreatedAt = DateTime.UtcNow;
        var expectedAverageGrade = 8.5;
        var expectedGrades = new List<GradeResponseDto>
        {
            new GradeResponseDto { Id = 1, Value = 8.5, Subject = "Math", StudentId = 1 },
        };

        // Act
        var dto = new StudentResponseDto
        {
            Id = expectedId,
            Name = expectedName,
            Email = expectedEmail,
            CreatedAt = expectedCreatedAt,
            AverageGrade = expectedAverageGrade,
        };

        foreach (var grade in expectedGrades)
        {
            dto.Grades.Add(grade);
        }

        // Assert
        dto.Id.Should().Be(expectedId);
        dto.Name.Should().Be(expectedName);
        dto.Email.Should().Be(expectedEmail);
        dto.CreatedAt.Should().Be(expectedCreatedAt);
        dto.AverageGrade.Should().Be(expectedAverageGrade);
        dto.Grades.Should().BeEquivalentTo(expectedGrades);
    }

    [Fact]
    public void GradeResponseDto_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var dto = new GradeResponseDto();

        // Assert
        dto.Id.Should().Be(0);
        dto.Value.Should().Be(0.0);
        dto.Subject.Should().Be(string.Empty);
        dto.CreatedAt.Should().Be(default);
        dto.StudentId.Should().Be(0);
    }

    [Fact]
    public void GradeResponseDto_WithProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedId = 1;
        var expectedValue = 9.0;
        var expectedSubject = "Physics";
        var expectedCreatedAt = DateTime.UtcNow;
        var expectedStudentId = 1;

        // Act
        var dto = new GradeResponseDto
        {
            Id = expectedId,
            Value = expectedValue,
            Subject = expectedSubject,
            CreatedAt = expectedCreatedAt,
            StudentId = expectedStudentId,
        };

        // Assert
        dto.Id.Should().Be(expectedId);
        dto.Value.Should().Be(expectedValue);
        dto.Subject.Should().Be(expectedSubject);
        dto.CreatedAt.Should().Be(expectedCreatedAt);
        dto.StudentId.Should().Be(expectedStudentId);
    }
}
