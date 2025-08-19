using FluentAssertions;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Models;

public class StudentConstructorTests
{
    [Fact]
    public void Student_DefaultConstructor_ShouldInitializeGradesCollection()
    {
        // Act
        var student = new Student();

        // Assert
        student.Should().NotBeNull();
        student.Grades.Should().NotBeNull();
        student.Grades.Should().BeEmpty();
    }

    [Fact]
    public void Student_ParameterizedConstructor_ShouldSetPropertiesAndInitializeGrades()
    {
        // Arrange
        const string Name = "Test Student";
        const string Email = "test@example.com";

        // Act
        var student = new Student
        {
            Name = Name,
            Email = Email,
        };

        // Assert
        student.Should().NotBeNull();
        student.Name.Should().Be(Name);
        student.Email.Should().Be(Email);
        student.Grades.Should().NotBeNull();
        student.Grades.Should().BeEmpty();
    }

    [Fact]
    public void Student_Grades_ShouldBeModifiableCollection()
    {
        // Arrange
        var student = new Student();
        var grade = new Grade
        {
            Subject = "Math",
            Value = 8.5,
            CreatedAt = DateTime.UtcNow,
            StudentId = student.Id,
        };

        // Act
        student.Grades.Add(grade);

        // Assert
        student.Grades.Should().HaveCount(1);
        student.Grades.Should().Contain(grade);
    }

    [Fact]
    public void Student_AverageGrade_WithEmptyGrades_ShouldReturnZero()
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            Email = "test@example.com",
        };

        // Act
        var averageGrade = student.AverageGrade;

        // Assert
        averageGrade.Should().Be(0.0);
    }

    [Fact]
    public void Student_AverageGrade_WithGrades_ShouldCalculateCorrectly()
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            Email = "test@example.com",
        };

        student.Grades.Add(new Grade { Subject = "Math", Value = 8.0, CreatedAt = DateTime.UtcNow, StudentId = student.Id });
        student.Grades.Add(new Grade { Subject = "Science", Value = 9.0, CreatedAt = DateTime.UtcNow, StudentId = student.Id });

        // Act
        var averageGrade = student.AverageGrade;

        // Assert
        averageGrade.Should().Be(8.5);
    }
}