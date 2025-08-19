using System.ComponentModel.DataAnnotations;

namespace StudentGradesAPI.Models;

public sealed class Student
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property for grades
    public ICollection<Grade> Grades { get; } = new List<Grade>();

    // Calculated property for average grade
    public double AverageGrade => Grades.Count != 0 ? Grades.Average(g => g.Value) : 0.0;
}
