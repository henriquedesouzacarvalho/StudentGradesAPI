using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudentGradesAPI.Models;
using StudentGradesAPI.Tests.Helpers;
using Xunit;

namespace StudentGradesAPI.Tests.Models;

public sealed class StudentGradesContextTests : IDisposable
{
    private readonly StudentGradesContext _context;

    public StudentGradesContextTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
    }

    [Fact]
    public void Context_ShouldHaveStudentsDbSet()
    {
        // Assert
        _context.Students.Should().NotBeNull();
    }

    [Fact]
    public void Context_ShouldHaveGradesDbSet()
    {
        // Assert
        _context.Grades.Should().NotBeNull();
    }

    [Fact]
    public async Task Context_ShouldCreateStudent()
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            Email = "test@example.com"
        };

        // Act
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        // Assert
        var savedStudent = await _context.Students.FirstOrDefaultAsync(s => s.Email == "test@example.com");
        savedStudent.Should().NotBeNull();
        savedStudent!.Name.Should().Be("Test Student");
        savedStudent.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task Context_ShouldCreateGrade()
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            Email = "test@example.com",
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var grade = new Grade
        {
            Value = 8.5,
            Subject = "Mathematics",
            StudentId = student.Id,
        };

        // Act
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        // Assert
        var savedGrade = await _context.Grades
            .Include(g => g.Student)
            .FirstOrDefaultAsync(g => g.Subject == "Mathematics");

        savedGrade.Should().NotBeNull();
        savedGrade!.Value.Should().Be(8.5);
        savedGrade.Subject.Should().Be("Mathematics");
        savedGrade.Student.Should().NotBeNull();
        savedGrade.Student.Name.Should().Be("Test Student");
    }

    [Fact]
    public async Task Context_ShouldAllowUniqueEmails()
    {
        // Arrange
        var student1 = new Student
        {
            Name = "Student 1",
            Email = "student1@example.com"
        };
        var student2 = new Student
        {
            Name = "Student 2",
            Email = "student2@example.com"
        };

        // Act
        _context.Students.Add(student1);
        _context.Students.Add(student2);
        await _context.SaveChangesAsync();

        // Assert
        var students = await _context.Students.ToListAsync();
        students.Should().HaveCount(2);
        students.Should().Contain(s => s.Email == "student1@example.com");
        students.Should().Contain(s => s.Email == "student2@example.com");
    }

    [Fact]
    public async Task Context_ShouldCascadeDeleteGradesWhenStudentDeleted()
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            Email = "test@example.com"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var grade = new Grade
        {
            Value = 8.5,
            Subject = "Mathematics",
            StudentId = student.Id
        };
        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        // Act
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        // Assert
        var remainingGrades = await _context.Grades.Where(g => g.StudentId == student.Id).ToListAsync();
        remainingGrades.Should().BeEmpty();
    }

    [Fact]
    public async Task Context_ShouldLoadStudentWithGrades()
    {
        // Arrange
        var student = new Student
        {
            Name = "Test Student",
            Email = "test@example.com"
        };
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        var grades = new List<Grade>
        {
            new Grade { Value = 8.5, Subject = "Math", StudentId = student.Id },
            new Grade { Value = 9.0, Subject = "Physics", StudentId = student.Id }
        };
        _context.Grades.AddRange(grades);
        await _context.SaveChangesAsync();

        // Act
        var loadedStudent = await _context.Students
            .Include(s => s.Grades)
            .FirstOrDefaultAsync(s => s.Id == student.Id);

        // Assert
        loadedStudent.Should().NotBeNull();
        loadedStudent!.Grades.Should().HaveCount(2);
        loadedStudent.Grades.Should().Contain(g => g.Subject == "Math" && g.Value == 8.5);
        loadedStudent.Grades.Should().Contain(g => g.Subject == "Physics" && g.Value == 9.0);
    }

    [Fact]
    public void Context_ShouldHaveCorrectDatabaseProvider()
    {
        // Act
        var providerName = _context.Database.ProviderName;

        // Assert
        providerName.Should().Contain("InMemory");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
