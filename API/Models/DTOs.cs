using System.ComponentModel.DataAnnotations;

namespace StudentGradesAPI.Models;

// DTO for creating a new student
public sealed class CreateStudentDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

// DTO for updating a student
public sealed class UpdateStudentDto
{
    [StringLength(100)]
    public string? Name { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
}

// DTO for creating a new grade
public sealed class CreateGradeDto
{
    [Required]
    [Range(0, 10, ErrorMessage = "Grade must be between 0 and 10")]
    public double Value { get; set; }

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "StudentId is required")]
    public int StudentId { get; set; }
}

// DTO for updating a grade
public sealed class UpdateGradeDto
{
    [Range(0, 10, ErrorMessage = "Grade must be between 0 and 10")]
    public double? Value { get; set; }

    [StringLength(100)]
    public string? Subject { get; set; }
}

// DTO for student response with average
public sealed class StudentResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public double AverageGrade { get; set; }

    public IList<GradeResponseDto> Grades { get; } = new List<GradeResponseDto>();
}

// DTO for grade response
public sealed class GradeResponseDto
{
    public int Id { get; set; }

    public double Value { get; set; }

    public string Subject { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int StudentId { get; set; }
}
