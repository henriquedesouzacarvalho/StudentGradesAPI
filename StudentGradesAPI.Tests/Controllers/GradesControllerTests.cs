using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StudentGradesAPI.Controllers;
using StudentGradesAPI.Models;
using StudentGradesAPI.Tests.Helpers;
using Xunit;

namespace StudentGradesAPI.Tests.Controllers;

public sealed class GradesControllerTests : IDisposable
{
    private readonly StudentGradesContext _context;
    private readonly GradesController _controller;

    public GradesControllerTests()
    {
        _context = TestDbContextFactory.CreateContextWithData();
        _controller = new GradesController(_context);
    }

    [Fact]
    public async Task GetGrades_ShouldReturnAllGrades()
    {
        // Act
        var result = await _controller.GetGrades();

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var grades = actionResult.Value.Should().BeAssignableTo<IEnumerable<GradeResponseDto>>().Subject;
        grades.Should().HaveCount(3);

        var gradesList = grades.ToList();
        gradesList.Should().Contain(g => g.Subject == "Mathematics" && g.Value == 8.5);
        gradesList.Should().Contain(g => g.Subject == "Physics" && g.Value == 9.0);
        gradesList.Should().Contain(g => g.Subject == "Chemistry" && g.Value == 7.5);
    }

    [Fact]
    public async Task GetGrade_WithValidId_ShouldReturnGrade()
    {
        // Act
        var result = await _controller.GetGrade(1);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var grade = actionResult.Value.Should().BeOfType<GradeResponseDto>().Subject;

        grade.Id.Should().Be(1);
        grade.Value.Should().Be(8.5);
        grade.Subject.Should().Be("Mathematics");
        grade.StudentId.Should().Be(1);
    }

    [Fact]
    public async Task GetGrade_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.GetGrade(999);

        // Assert
        var actionResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Grade with ID 999 not found." });
    }

    [Fact]
    public async Task GetGradesByStudent_WithValidStudentId_ShouldReturnGradesAndAverage()
    {
        // Act
        var result = await _controller.GetGradesByStudent(1);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;

        // Use anonymous type matching
        var expectedResponse = new
        {
            StudentId = 1,
            StudentName = "John Doe",
            Grades = new List<object>(),
            AverageGrade = 8.75,
            TotalGrades = 2,
        };

        response.Should().BeEquivalentTo(expectedResponse, options => options
            .Excluding(x => x.Grades)); // We'll check grades separately

        // Check grades separately
        var responseType = response.GetType();
        var gradesProperty = responseType.GetProperty("Grades");
        var grades = gradesProperty!.GetValue(response) as IEnumerable<GradeResponseDto>;
        grades.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetGradesByStudent_WithInvalidStudentId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.GetGradesByStudent(999);

        // Assert
        var actionResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Student with ID 999 not found." });
    }

    [Fact]
    public async Task GetGradesByStudent_WithStudentWithoutGrades_ShouldReturnZeroAverage()
    {
        // Arrange - Create a student without grades
        var student = new Student
        {
            Name = "No Grades Student",
            Email = "nogrades@example.com",
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetGradesByStudent(student.Id);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;

        var expectedResponse = new
        {
            StudentId = student.Id,
            StudentName = "No Grades Student",
            Grades = new List<object>(),
            AverageGrade = 0.0,
            TotalGrades = 0,
        };

        response.Should().BeEquivalentTo(expectedResponse, options => options
            .Excluding(x => x.Grades));
    }

    [Fact]
    public async Task PostGrade_WithValidData_ShouldCreateGrade()
    {
        // Arrange
        var createDto = new CreateGradeDto
        {
            Value = 9.5,
            Subject = "Biology",
            StudentId = 1,
        };

        // Act
        var result = await _controller.PostGrade(createDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        actionResult.ActionName.Should().Be(nameof(GradesController.GetGrade));

        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;

        // Verify the response structure
        var responseType = response.GetType();
        var gradeProperty = responseType.GetProperty("Grade");
        var grade = gradeProperty!.GetValue(response) as GradeResponseDto;

        grade.Should().NotBeNull();
        grade!.Value.Should().Be(9.5);
        grade.Subject.Should().Be("Biology");
        grade.StudentId.Should().Be(1);
    }

    [Fact]
    public async Task PostGrade_WithInvalidStudentId_ShouldReturnBadRequest()
    {
        // Arrange
        var createDto = new CreateGradeDto
        {
            Value = 8.0,
            Subject = "History",
            StudentId = 999, // Non-existent student
        };

        // Act
        var result = await _controller.PostGrade(createDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Student with ID 999 not found." });
    }

    [Fact]
    public async Task PutGrade_WithValidData_ShouldUpdateGrade()
    {
        // Arrange
        var updateDto = new UpdateGradeDto
        {
            Value = 9.5,
            Subject = "Advanced Mathematics",
        };

        // Act
        var result = await _controller.PutGrade(1, updateDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;

        // Verify the response structure
        var responseType = response.GetType();
        var gradeProperty = responseType.GetProperty("Grade");
        var grade = gradeProperty!.GetValue(response) as GradeResponseDto;

        grade.Should().NotBeNull();
        grade!.Id.Should().Be(1);
        grade.Value.Should().Be(9.5);
        grade.Subject.Should().Be("Advanced Mathematics");
    }

    [Fact]
    public async Task PutGrade_WithPartialData_ShouldUpdateOnlyProvidedFields()
    {
        // Arrange
        var updateDto = new UpdateGradeDto
        {
            Value = 7.0,
            // Subject is null, should not be updated
        };

        // Act
        var result = await _controller.PutGrade(1, updateDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;

        var responseType = response.GetType();
        var gradeProperty = responseType.GetProperty("Grade");
        var grade = gradeProperty!.GetValue(response) as GradeResponseDto;

        grade.Should().NotBeNull();
        grade!.Value.Should().Be(7.0);
        grade.Subject.Should().Be("Mathematics"); // Should remain unchanged
    }

    [Fact]
    public async Task PutGrade_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var updateDto = new UpdateGradeDto
        {
            Value = 8.0,
        };

        // Act
        var result = await _controller.PutGrade(999, updateDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Grade with ID 999 not found." });
    }

    [Fact]
    public async Task DeleteGrade_WithValidId_ShouldDeleteGrade()
    {
        // Act
        var result = await _controller.DeleteGrade(1);

        // Assert
        var actionResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;

        // Verify the response contains the expected message
        var responseType = response.GetType();
        var messageProperty = responseType.GetProperty("Message");
        var message = messageProperty!.GetValue(response) as string;
        message.Should().Contain("Grade with ID 1 has been deleted successfully");

        // Verify grade is actually deleted
        var getResult = await _controller.GetGrade(1);
        getResult.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task DeleteGrade_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var result = await _controller.DeleteGrade(999);

        // Assert
        var actionResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;
        response.Should().BeEquivalentTo(new { message = "Grade with ID 999 not found." });
    }

    [Fact]
    public async Task PostGrade_ShouldUpdateStudentAverage()
    {
        // Arrange - Student 1 currently has grades 8.5 and 9.0 (average 8.75)
        var createDto = new CreateGradeDto
        {
            Value = 6.0, // This should lower the average
            Subject = "Art",
            StudentId = 1,
        };

        // Act
        var result = await _controller.PostGrade(createDto);

        // Assert
        var actionResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = actionResult.Value.Should().BeAssignableTo<object>().Subject;

        var responseType = response.GetType();
        var studentInfoProperty = responseType.GetProperty("StudentInfo");
        var studentInfo = studentInfoProperty!.GetValue(response);

        var studentInfoType = studentInfo!.GetType();
        var newAverageProperty = studentInfoType.GetProperty("NewAverageGrade");
        var newAverage = (double)newAverageProperty!.GetValue(studentInfo)!;

        // New average should be (8.5 + 9.0 + 6.0) / 3 = 7.83 (rounded to 2 decimal places)
        newAverage.Should().Be(7.83);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
