using FluentAssertions;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Models;

public class StudentTests
{
    [Fact]
    public void Student_DefaultConstructor_ShouldInitializeProperties()
    {
        // Act
        var student = new Student();

        // Assert
        student.Id.Should().Be(0);
        student.Name.Should().Be(string.Empty);
        student.Email.Should().Be(string.Empty);
        student.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        student.Grades.Should().NotBeNull().And.BeEmpty();
        student.AverageGrade.Should().Be(0.0);
    }

    [Fact]
    public void Student_WithProperties_ShouldSetCorrectly()
    {
        // Arrange
        var expectedName = "John Doe";
        var expectedEmail = "john.doe@example.com";
        var expectedCreatedAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var student = new Student
        {
            Id = 1,
            Name = expectedName,
            Email = expectedEmail,
            CreatedAt = expectedCreatedAt
        };

        // Assert
        student.Id.Should().Be(1);
        student.Name.Should().Be(expectedName);
        student.Email.Should().Be(expectedEmail);
        student.CreatedAt.Should().Be(expectedCreatedAt);
        student.Grades.Should().NotBeNull().And.BeEmpty();
        student.AverageGrade.Should().Be(0.0);
    }

    [Fact]
    public void AverageGrade_WithNoGrades_ShouldReturnZero()
    {
        // Arrange
        var student = new Student
        {
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        // Act & Assert
        student.AverageGrade.Should().Be(0.0);
    }

    [Fact]
    public void AverageGrade_WithGrades_ShouldCalculateCorrectAverage()
    {
        // Arrange
        var student = new Student
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Grades = new List<Grade>
            {
                new Grade { Value = 8.0, Subject = "Math", StudentId = 1 },
                new Grade { Value = 9.0, Subject = "Physics", StudentId = 1 },
                new Grade { Value = 7.0, Subject = "Chemistry", StudentId = 1 }
            }
        };

        // Act
        var average = student.AverageGrade;

        // Assert
        average.Should().Be(8.0); // (8.0 + 9.0 + 7.0) / 3 = 8.0
    }

    [Fact]
    public void AverageGrade_WithSingleGrade_ShouldReturnThatGrade()
    {
        // Arrange
        var student = new Student
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Grades = new List<Grade>
            {
                new Grade { Value = 8.5, Subject = "Math", StudentId = 1 }
            }
        };

        // Act
        var average = student.AverageGrade;

        // Assert
        average.Should().Be(8.5);
    }

    [Fact]
    public void Grades_Collection_ShouldBeInitializedAsEmptyList()
    {
        // Arrange & Act
        var student = new Student();

        // Assert
        student.Grades.Should().NotBeNull();
        student.Grades.Should().BeOfType<List<Grade>>();
        student.Grades.Should().BeEmpty();
    }

    [Fact]
    public void Grades_Collection_ShouldAllowAddingGrades()
    {
        // Arrange
        var student = new Student
        {
            Name = "John Doe",
            Email = "john.doe@example.com"
        };
        var grade = new Grade
        {
            Value = 8.5,
            Subject = "Mathematics",
            StudentId = 1
        };

        // Act
        student.Grades.Add(grade);

        // Assert
        student.Grades.Should().HaveCount(1);
        student.Grades.First().Should().Be(grade);
        student.AverageGrade.Should().Be(8.5);
    }
}