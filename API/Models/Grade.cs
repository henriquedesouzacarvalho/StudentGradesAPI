using System.ComponentModel.DataAnnotations;

namespace StudentGradesAPI.Models;

public class Grade
{
    public int Id { get; set; }

    [Required]
    [Range(0.0, 10.0, ErrorMessage = "Grade must be between 0 and 10")]
    public double Value { get; set; }

    [Required]
    [StringLength(100)]
    public string Subject { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int StudentId { get; set; }

    // Navigation property
    public virtual Student Student { get; set; } = null!;
}
