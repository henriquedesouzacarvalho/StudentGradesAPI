using FluentAssertions;
using StudentGradesAPI.Extensions;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Extensions;

public class MappingExtensionsTests
{
    [Fact]
    public void ToResponseDto_WithStudent_ShouldMapCorrectly()
    {
        // Arrange
        var student = new Student
        {
            Id = 1,
            Name = "John Doe",
            Email = "john.doe@example.com",
            CreatedAt = DateTime.UtcNow,
        };

        student.Grades.Add(new() { Id = 1, Value = 8.5, Subject = "Math", StudentId = 1, CreatedAt = DateTime.UtcNow });
        student.Grades.Add(new() { Id = 2, Value = 9.0, Subject = "Physics", StudentId = 1, CreatedAt = DateTime.UtcNow });

        // Act
        var result = student.ToResponseDto();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(student.Id);
        result.Name.Should().Be(student.Name);
        result.Email.Should().Be(student.Email);
        result.CreatedAt.Should().Be(student.CreatedAt);
        result.AverageGrade.Should().Be(student.AverageGrade);
        result.Grades.Should().HaveCount(2);
        result.Grades.First().Id.Should().Be(1);
        result.Grades.First().Value.Should().Be(8.5);
        result.Grades.First().Subject.Should().Be("Math");
    }

    [Fact]
    public void ToResponseDto_WithNullStudent_ShouldThrowArgumentNullException()
    {
        // Arrange
        Student student = null!;

        // Act & Assert
        var action = student.ToResponseDto;
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToResponseDto_WithGrade_ShouldMapCorrectly()
    {
        // Arrange
        var grade = new Grade
        {
            Id = 1,
            Value = 8.5,
            Subject = "Mathematics",
            StudentId = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = grade.ToResponseDto();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(grade.Id);
        result.Value.Should().Be(grade.Value);
        result.Subject.Should().Be(grade.Subject);
        result.StudentId.Should().Be(grade.StudentId);
        result.CreatedAt.Should().Be(grade.CreatedAt);
    }

    [Fact]
    public void ToResponseDto_WithNullGrade_ShouldThrowArgumentNullException()
    {
        // Arrange
        Grade grade = null!;

        // Act & Assert
        var action = grade.ToResponseDto;
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToResponseDto_WithStudentCollection_ShouldMapAllStudents()
    {
        // Arrange
        var students = new List<Student>
        {
            new() { Id = 1, Name = "John", Email = "john@example.com", CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Jane", Email = "jane@example.com", CreatedAt = DateTime.UtcNow }
        };

        // Act
        var result = students.ToResponseDto().ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("John");
        result[1].Name.Should().Be("Jane");
    }

    [Fact]
    public void ToResponseDto_WithNullStudentCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Student> students = null!;

        // Act & Assert
        var action = students.ToResponseDto;
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToResponseDto_WithGradeCollection_ShouldMapAllGrades()
    {
        // Arrange
        var grades = new List<Grade>
        {
            new() { Id = 1, Value = 8.5, Subject = "Math", StudentId = 1, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Value = 9.0, Subject = "Physics", StudentId = 1, CreatedAt = DateTime.UtcNow }
        };

        // Act
        var result = grades.ToResponseDto().ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Subject.Should().Be("Math");
        result[1].Subject.Should().Be("Physics");
    }

    [Fact]
    public void ToResponseDto_WithNullGradeCollection_ShouldThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<Grade> grades = null!;

        // Act & Assert
        var action = grades.ToResponseDto;
        action.Should().Throw<ArgumentNullException>();
    }
}