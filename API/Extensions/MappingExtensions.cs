namespace StudentGradesAPI.Extensions;

using StudentGradesAPI.Models;

public static class MappingExtensions
{
    public static StudentResponseDto ToResponseDto(this Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        var dto = new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            CreatedAt = student.CreatedAt,
            AverageGrade = student.AverageGrade,
        };

        foreach (var grade in student.Grades)
        {
            dto.Grades.Add(grade.ToResponseDto());
        }

        return dto;
    }

    public static GradeResponseDto ToResponseDto(this Grade grade)
    {
        ArgumentNullException.ThrowIfNull(grade);

        return new GradeResponseDto
        {
            Id = grade.Id,
            Value = grade.Value,
            Subject = grade.Subject,
            CreatedAt = grade.CreatedAt,
            StudentId = grade.StudentId,
        };
    }

    public static IEnumerable<StudentResponseDto> ToResponseDto(this IEnumerable<Student> students)
    {
        ArgumentNullException.ThrowIfNull(students);

        return students.Select(s => s.ToResponseDto());
    }

    public static IEnumerable<GradeResponseDto> ToResponseDto(this IEnumerable<Grade> grades)
    {
        ArgumentNullException.ThrowIfNull(grades);

        return grades.Select(g => g.ToResponseDto());
    }
}