using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradesAPI.Models;

namespace StudentGradesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly StudentGradesContext _context;

    public StudentsController(StudentGradesContext context)
    {
        _context = context;
    }

    // GET: api/Students
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetStudents()
    {
        var students = await _context.Students
            .Include(s => s.Grades)
            .Select(s => new StudentResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                CreatedAt = s.CreatedAt,
                AverageGrade = s.Grades.Any() ? s.Grades.Average(g => g.Value) : 0.0,
                Grades = s.Grades.Select(g => new GradeResponseDto
                {
                    Id = g.Id,
                    Value = g.Value,
                    Subject = g.Subject,
                    CreatedAt = g.CreatedAt,
                    StudentId = g.StudentId
                }).ToList(),
            })
            .ToListAsync();

        return Ok(students);
    }

    // GET: api/Students/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentResponseDto>> GetStudent(int id)
    {
        var student = await _context.Students
            .Include(s => s.Grades)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found." });
        }

        var studentDto = new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            CreatedAt = student.CreatedAt,
            AverageGrade = student.Grades.Any() ? student.Grades.Average(g => g.Value) : 0.0,
            Grades = student.Grades.Select(g => new GradeResponseDto
            {
                Id = g.Id,
                Value = g.Value,
                Subject = g.Subject,
                CreatedAt = g.CreatedAt,
                StudentId = g.StudentId
            }).ToList(),
        };

        return Ok(studentDto);
    }

    // POST: api/Students
    [HttpPost]
    public async Task<ActionResult<StudentResponseDto>> PostStudent(CreateStudentDto createStudentDto)
    {
        // Check if email already exists
        if (await _context.Students.AnyAsync(s => s.Email == createStudentDto.Email))
        {
            return BadRequest(new { message = "A student with this email already exists." });
        }

        var student = new Student
        {
            Name = createStudentDto.Name,
            Email = createStudentDto.Email,
            CreatedAt = DateTime.UtcNow,
        };

        _ = _context.Students.Add(student);
        _ = await _context.SaveChangesAsync();

        var studentDto = new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            CreatedAt = student.CreatedAt,
            AverageGrade = 0.0,
            Grades = new List<GradeResponseDto>(),
        };

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, studentDto);
    }

    // PUT: api/Students/5
    [HttpPut("{id}")]
    public async Task<ActionResult<StudentResponseDto>> PutStudent(int id, UpdateStudentDto updateStudentDto)
    {
        var student = await _context.Students
            .Include(s => s.Grades)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found." });
        }

        // Check if email already exists for another student
        if (!string.IsNullOrEmpty(updateStudentDto.Email) &&
            await _context.Students.AnyAsync(s => s.Email == updateStudentDto.Email && s.Id != id))
        {
            return BadRequest(new { message = "A student with this email already exists." });
        }

        // Update only provided fields
        if (!string.IsNullOrEmpty(updateStudentDto.Name))
        {
            student.Name = updateStudentDto.Name;
        }

        if (!string.IsNullOrEmpty(updateStudentDto.Email))
        {
            student.Email = updateStudentDto.Email;
        }

        _ = await _context.SaveChangesAsync();

        var studentDto = new StudentResponseDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            CreatedAt = student.CreatedAt,
            AverageGrade = student.Grades.Any() ? student.Grades.Average(g => g.Value) : 0.0,
            Grades = student.Grades.Select(g => new GradeResponseDto
            {
                Id = g.Id,
                Value = g.Value,
                Subject = g.Subject,
                CreatedAt = g.CreatedAt,
                StudentId = g.StudentId
            }).ToList(),
        };

        return Ok(studentDto);
    }

    // DELETE: api/Students/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound(new { message = $"Student with ID {id} not found." });
        }

        _ = _context.Students.Remove(student);
        _ = await _context.SaveChangesAsync();

        return Ok(new { message = $"Student with ID {id} has been deleted successfully." });
    }
}
