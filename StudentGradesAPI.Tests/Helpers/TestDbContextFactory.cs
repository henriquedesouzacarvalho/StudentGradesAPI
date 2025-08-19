using Microsoft.EntityFrameworkCore;
using StudentGradesAPI.Models;

namespace StudentGradesAPI.Tests.Helpers;

public static class TestDbContextFactory
{
    public static StudentGradesContext CreateInMemoryContext(string databaseName = "")
    {
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = Guid.NewGuid().ToString();
        }

        var options = new DbContextOptionsBuilder<StudentGradesContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new StudentGradesContext(options);
    }

    public static StudentGradesContext CreateContextWithData(string databaseName = "")
    {
        var context = CreateInMemoryContext(databaseName);

        // Add test data
        var students = new List<Student>
        {
            new Student
            {
                Id = 1,
                Name = "John Doe",
                Email = "john.doe@example.com",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Student
            {
                Id = 2,
                Name = "Jane Smith",
                Email = "jane.smith@example.com",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        var grades = new List<Grade>
        {
            new Grade
            {
                Id = 1,
                Value = 8.5,
                Subject = "Mathematics",
                StudentId = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Grade
            {
                Id = 2,
                Value = 9.0,
                Subject = "Physics",
                StudentId = 1,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Grade
            {
                Id = 3,
                Value = 7.5,
                Subject = "Chemistry",
                StudentId = 2,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        context.Students.AddRange(students);
        context.Grades.AddRange(grades);
        context.SaveChanges();

        return context;
    }
}