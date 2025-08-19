using FluentAssertions;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Models;

public class GradeTests
{
    [Fact]
    public void Grade_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var grade = new Grade();

        // Assert
        grade.Id.Should().Be(0);
        grade.Value.Should().Be(0.0);
        grade.Subject.Should().Be(string.Empty);
        grade.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        grade.StudentId.Should().Be(0);
        grade.Student.Should().BeNull();
    }

    [Fact]
    public void Grade_WithProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedValue = 8.5;
        var expectedSubject = "Mathematics";
        var expectedStudentId = 1;
        var expectedCreatedAt = DateTime.UtcNow.AddDays(-1);
        var expectedStudent = new Student { Id = 1, Name = "John Doe", Email = "john@example.com" };

        // Act
        var grade = new Grade
        {
            Id = 1,
            Value = expectedValue,
            Subject = expectedSubject,
            StudentId = expectedStudentId,
            CreatedAt = expectedCreatedAt,
            Student = expectedStudent
        };

        // Assert
        grade.Id.Should().Be(1);
        grade.Value.Should().Be(expectedValue);
        grade.Subject.Should().Be(expectedSubject);
        grade.StudentId.Should().Be(expectedStudentId);
        grade.CreatedAt.Should().Be(expectedCreatedAt);
        grade.Student.Should().Be(expectedStudent);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(5.5)]
    [InlineData(10.0)]
    public void Grade_Value_ShouldAcceptValidRange(double value)
    {
        // Arrange & Act
        var grade = new Grade { Value = value };

        // Assert
        grade.Value.Should().Be(value);
    }

    [Fact]
    public void Grade_Subject_ShouldAcceptValidString()
    {
        // Arrange
        var expectedSubject = "Computer Science";

        // Act
        var grade = new Grade { Subject = expectedSubject };

        // Assert
        grade.Subject.Should().Be(expectedSubject);
    }

    [Fact]
    public void Grade_StudentId_ShouldSetCorrectly()
    {
        // Arrange
        var expectedStudentId = 42;

        // Act
        var grade = new Grade { StudentId = expectedStudentId };

        // Assert
        grade.StudentId.Should().Be(expectedStudentId);
    }

    [Fact]
    public void Grade_CreatedAt_ShouldBeSettable()
    {
        // Arrange
        var expectedDate = DateTime.UtcNow.AddDays(-5);

        // Act
        var grade = new Grade { CreatedAt = expectedDate };

        // Assert
        grade.CreatedAt.Should().Be(expectedDate);
    }

    [Fact]
    public void Grade_Student_NavigationProperty_ShouldWork()
    {
        // Arrange
        var student = new Student
        {
            Id = 1,
            Name = "Jane Doe",
            Email = "jane@example.com"
        };

        // Act
        var grade = new Grade
        {
            Value = 9.0,
            Subject = "Physics",
            StudentId = student.Id,
            Student = student
        };

        // Assert
        grade.Student.Should().Be(student);
        grade.Student.Id.Should().Be(student.Id);
        grade.Student.Name.Should().Be(student.Name);
        grade.Student.Email.Should().Be(student.Email);
    }
}