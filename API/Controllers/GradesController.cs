using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradesAPI.Extensions;
using StudentGradesAPI.Models;

namespace StudentGradesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GradesController : ControllerBase
{
    private readonly StudentGradesContext _context;

    public GradesController(StudentGradesContext context)
    {
        _context = context;
    }

    // GET: api/Grades
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GradeResponseDto>>> GetGrades()
    {
        var grades = await _context.Grades
            .Include(g => g.Student)
            .ToListAsync();

        return Ok(grades.ToResponseDto());
    }

    // GET: api/Grades/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GradeResponseDto>> GetGrade(int id)
    {
        var grade = await _context.Grades
            .Include(g => g.Student)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grade == null)
        {
            return this.EntityNotFound("Grade", id);
        }

        return Ok(grade.ToResponseDto());
    }

    // GET: api/Grades/student/5
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<object>> GetGradesByStudent(int studentId)
    {
        var student = await _context.Students
            .Include(s => s.Grades)
            .FirstOrDefaultAsync(s => s.Id == studentId);

        if (student == null)
        {
            return this.EntityNotFound("Student", studentId);
        }

        var grades = student.Grades.Select(g => new GradeResponseDto
        {
            Id = g.Id,
            Value = g.Value,
            Subject = g.Subject,
            CreatedAt = g.CreatedAt,
            StudentId = g.StudentId,
        }).ToList();

        var averageGrade = grades.Count != 0 ? grades.Average(g => g.Value) : 0.0;

        return Ok(new
        {
            StudentId = studentId,
            StudentName = student.Name,
            Grades = grades,
            AverageGrade = Math.Round(averageGrade, 2),
            TotalGrades = grades.Count,
        });
    }

    // POST: api/Grades
    [HttpPost]
    public async Task<ActionResult<object>> PostGrade(CreateGradeDto createGradeDto)
    {
        ArgumentNullException.ThrowIfNull(createGradeDto);

        // Check if student exists
        var student = await _context.Students
            .Include(s => s.Grades)
            .FirstOrDefaultAsync(s => s.Id == createGradeDto.StudentId);

        if (student == null)
        {
            return BadRequest(new { message = $"Student with ID {createGradeDto.StudentId} not found." });
        }

        var grade = new Grade
        {
            Value = createGradeDto.Value,
            Subject = createGradeDto.Subject,
            StudentId = createGradeDto.StudentId,
            CreatedAt = DateTime.UtcNow,
        };

        _ = _context.Grades.Add(grade);
        _ = await _context.SaveChangesAsync();

        // Reload student with updated grades to calculate new average
        await _context.Entry(student).ReloadAsync();
        await _context.Entry(student).Collection(s => s.Grades).LoadAsync();

        var newAverageGrade = student.Grades.Count != 0 ? student.Grades.Average(g => g.Value) : 0.0;

        var gradeDto = new GradeResponseDto
        {
            Id = grade.Id,
            Value = grade.Value,
            Subject = grade.Subject,
            CreatedAt = grade.CreatedAt,
            StudentId = grade.StudentId,
        };

        // Return grade with updated average
        var response = new
        {
            Grade = gradeDto,
            StudentInfo = new
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                NewAverageGrade = Math.Round(newAverageGrade, 2),
                TotalGrades = student.Grades.Count,
            },
            Message = $"Grade added successfully. New average: {Math.Round(newAverageGrade, 2)}",
        };

        return CreatedAtAction(nameof(GetGrade), new { id = grade.Id }, response);
    }

    // PUT: api/Grades/5
    [HttpPut("{id}")]
    public async Task<ActionResult<object>> PutGrade(int id, UpdateGradeDto updateGradeDto)
    {
        ArgumentNullException.ThrowIfNull(updateGradeDto);
        var grade = await _context.Grades
            .Include(g => g.Student)
            .ThenInclude(s => s.Grades)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grade == null)
        {
            return this.EntityNotFound("Grade", id);
        }

        // Update only provided fields
        if (updateGradeDto.Value.HasValue)
        {
            grade.Value = updateGradeDto.Value.Value;
        }

        if (!string.IsNullOrEmpty(updateGradeDto.Subject))
        {
            grade.Subject = updateGradeDto.Subject;
        }

        _ = await _context.SaveChangesAsync();

        // Calculate new average
        var newAverageGrade = grade.Student.Grades.Count != 0 ? grade.Student.Grades.Average(g => g.Value) : 0.0;

        var gradeDto = new GradeResponseDto
        {
            Id = grade.Id,
            Value = grade.Value,
            Subject = grade.Subject,
            CreatedAt = grade.CreatedAt,
            StudentId = grade.StudentId,
        };

        var response = new
        {
            Grade = gradeDto,
            StudentInfo = new
            {
                Id = grade.Student.Id,
                Name = grade.Student.Name,
                Email = grade.Student.Email,
                NewAverageGrade = Math.Round(newAverageGrade, 2),
                TotalGrades = grade.Student.Grades.Count,
            },
            Message = $"Grade updated successfully. New average: {Math.Round(newAverageGrade, 2)}",
        };

        return Ok(response);
    }

    // DELETE: api/Grades/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<object>> DeleteGrade(int id)
    {
        var grade = await _context.Grades
            .Include(g => g.Student)
            .ThenInclude(s => s.Grades)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grade == null)
        {
            return this.EntityNotFound("Grade", id);
        }

        var student = grade.Student;
        _ = _context.Grades.Remove(grade);
        _ = await _context.SaveChangesAsync();

        // Reload student grades and calculate new average
        await _context.Entry(student).Collection(s => s.Grades).LoadAsync();
        var newAverageGrade = student.Grades.Count != 0 ? student.Grades.Average(g => g.Value) : 0.0;

        var response = new
        {
            StudentInfo = new
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                NewAverageGrade = Math.Round(newAverageGrade, 2),
                TotalGrades = student.Grades.Count,
            },
            Message = $"Grade with ID {id} has been deleted successfully. New average: {Math.Round(newAverageGrade, 2)}",
        };

        return Ok(response);
    }
}
