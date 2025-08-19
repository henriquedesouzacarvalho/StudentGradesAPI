using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Models;

public class ValidationTests
{
    [Fact]
    public void Student_WithInvalidEmail_ShouldFailValidation()
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            Email = "invalid-email",
        };

        var context = new ValidationContext(student);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(student, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void Student_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            Email = "test@example.com",
        };

        var context = new ValidationContext(student);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(student, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void Student_WithNameTooLong_ShouldFailValidation()
    {
        // Arrange
        var longName = new string('A', 201); // Exceeds 200 character limit
        var student = new Student
        {
            Name = longName,
            Email = "test@example.com",
        };

        var context = new ValidationContext(student);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(student, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Name"));
    }

    [Fact]
    public void Grade_WithValueOutOfRange_ShouldFailValidation()
    {
        // Arrange
        var grade = new Grade
        {
            StudentId = 1,
            Subject = "Math",
            Value = 15, // Exceeds range of 0-10
            CreatedAt = DateTime.UtcNow,
        };

        var context = new ValidationContext(grade);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(grade, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Value"));
    }

    [Fact]
    public void Grade_WithNegativeValue_ShouldFailValidation()
    {
        // Arrange
        var grade = new Grade
        {
            StudentId = 1,
            Subject = "Math",
            Value = -1, // Below minimum range
            CreatedAt = DateTime.UtcNow,
        };

        var context = new ValidationContext(grade);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(grade, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Value"));
    }

    [Fact]
    public void Grade_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var grade = new Grade
        {
            StudentId = 1,
            Subject = "Math",
            Value = 8.5,
            CreatedAt = DateTime.UtcNow,
        };

        var context = new ValidationContext(grade);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(grade, context, results, true);

        // Assert
        isValid.Should().BeTrue();
        results.Should().BeEmpty();
    }

    [Fact]
    public void Grade_WithSubjectTooLong_ShouldFailValidation()
    {
        // Arrange
        var longSubject = new string('A', 101); // Exceeds 100 character limit
        var grade = new Grade
        {
            StudentId = 1,
            Subject = longSubject,
            Value = 8.5,
            CreatedAt = DateTime.UtcNow,
        };

        var context = new ValidationContext(grade);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(grade, context, results, true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains("Subject"));
    }
}