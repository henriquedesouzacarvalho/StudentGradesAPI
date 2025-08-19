using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StudentGradesAPI.Controllers;
using StudentGradesAPI.Models;
using StudentGradesAPI.Tests.Helpers;
using Xunit;

namespace StudentGradesAPI.Tests.Controllers;

public sealed class StudentsControllerTests : IDisposable
{
    private readonly StudentGradesContext _context;
    private readonly StudentsController _controller;

    public StudentsControllerTests()
    {
        _context = TestDbContextFactory.CreateContextWithData();
        _controller = new StudentsController(_context);
    }

    [Fact]
    public async Task GetStudents_ShouldReturnAllStudents()
    {
        // Act
        var result = await _controller.GetStudents();

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var students = actionResult.Value.Should().BeAssignableTo<IEnumerable<StudentResponseDto>>().Subject;
        students.Should().HaveCount(2);

        var studentsList = students.ToList();
        studentsList[0].Name.Should().Be("John Doe");
        studentsList[0].Email.Should().Be("john.doe@example.com");
        studentsList[0].Grades.Should().HaveCount(2);
        studentsList[0].AverageGrade.Should().Be(8.75); // (8.5 + 9.0) / 2

        studentsList[1].Name.Should().Be("Jane Smith");
        studentsList[1].Email.Should().Be("jane.smith@example.com");
        studentsList[1].Grades.Should().HaveCount(1);
        studentsList[1].AverageGrade.Should().Be(7.5);
    }

    [Fact]
    public async Task GetStudent_WithValidId_ShouldReturnStudent()
    {
        // Act
        var result = await _controller.GetStudent(1);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var student = actionResult.Value.Should().BeOfType<StudentResponseDto>().Subject;

        student.Id.Should().Be(1);
        student.Name.Should().Be("John Doe");
        student.Email.Should().Be("john.doe@example.com");
        student.Grades.Should().HaveCount(2);
        student.AverageGrade.Should().Be(8.75);
    }

    [Fact]
    public async Task GetStudent_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.GetStudent(999);

        // Assert
        var actionResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Student with ID 999 not found." });
    }

    [Fact]
    public async Task PostStudent_WithValidData_ShouldCreateStudent()
    {
        // Arrange
        var createDto = new CreateStudentDto
        {
            Name = "New Student",
            Email = "new.student@example.com",
        };

        // Act
        var result = await _controller.PostStudent(createDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var student = actionResult.Value.Should().BeOfType<StudentResponseDto>().Subject;

        student.Name.Should().Be("New Student");
        student.Email.Should().Be("new.student@example.com");
        student.AverageGrade.Should().Be(0.0);
        student.Grades.Should().BeEmpty();

        actionResult.ActionName.Should().Be(nameof(StudentsController.GetStudent));
        actionResult.RouteValues!["id"].Should().Be(student.Id);
    }

    [Fact]
    public async Task PostStudent_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateStudentDto
        {
            Name = "Duplicate Email",
            Email = "john.doe@example.com", // This email already exists
        };

        // Act
        var result = await _controller.PostStudent(createDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "A student with this email already exists." });
    }

    [Fact]
    public async Task PutStudent_WithValidData_ShouldUpdateStudent()
    {
        // Arrange
        var updateDto = new UpdateStudentDto
        {
            Name = "Updated Name",
            Email = "updated.email@example.com",
        };

        // Act
        var result = await _controller.PutStudent(1, updateDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var student = actionResult.Value.Should().BeOfType<StudentResponseDto>().Subject;

        student.Id.Should().Be(1);
        student.Name.Should().Be("Updated Name");
        student.Email.Should().Be("updated.email@example.com");
    }

    [Fact]
    public async Task PutStudent_WithPartialData_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var updateDto = new UpdateStudentDto
        {
            Name = "Only Name Updated",
            // Email is null, should not be updated
        };

        // Act
        var result = await _controller.PutStudent(1, updateDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var student = actionResult.Value.Should().BeOfType<StudentResponseDto>().Subject;

        student.Id.Should().Be(1);
        student.Name.Should().Be("Only Name Updated");
        student.Email.Should().Be("john.doe@example.com"); // Should remain unchanged
    }

    [Fact]
    public async Task PutStudent_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateStudentDto
        {
            Name = "Updated Name",
        };

        // Act
        var result = await _controller.PutStudent(999, updateDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Student with ID 999 not found." });
    }

    [Fact]
    public async Task PutStudent_WithDuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var updateDto = new UpdateStudentDto
        {
            Email = "jane.smith@example.com", // This email belongs to student ID 2
        };

        // Act
        var result = await _controller.PutStudent(1, updateDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "A student with this email already exists." });
    }

    [Fact]
    public async Task DeleteStudent_WithValidId_ShouldDeleteStudent()
    {
        // Act
        var result = await _controller.DeleteStudent(1);

        // Assert
        var actionResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Student with ID 1 has been deleted successfully." });

        // Verify student is actually deleted
        var getResult = await _controller.GetStudent(1);
        getResult.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteStudent_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.DeleteStudent(999);

        // Assert
        var actionResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Student with ID 999 not found." });
    }

    [Fact]
    public async Task PutStudent_WithSameEmail_ShouldAllowUpdate()
    {
        // Arrange - Update student with their own email (should be allowed)
        var updateDto = new UpdateStudentDto
        {
            Name = "Updated Name",
            Email = "john.doe@example.com", // Same email as student ID 1
        };

        // Act
        var result = await _controller.PutStudent(1, updateDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var student = actionResult.Value.Should().BeOfType<StudentResponseDto>().Subject;

        student.Id.Should().Be(1);
        student.Name.Should().Be("Updated Name");
        student.Email.Should().Be("john.doe@example.com");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
