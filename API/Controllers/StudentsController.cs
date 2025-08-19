using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradesAPI.Extensions;
using StudentGradesAPI.Models;

namespace StudentGradesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class StudentsController : ControllerBase
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
            .ToListAsync();

        return Ok(students.ToResponseDto());
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
            return this.EntityNotFound("Student", id);
        }

        return Ok(student.ToResponseDto());
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
            return this.EntityNotFound("Student", id);
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

        return Ok(student.ToResponseDto());
    }

    // DELETE: api/Students/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return this.EntityNotFound("Student", id);
        }

        _ = _context.Students.Remove(student);
        _ = await _context.SaveChangesAsync();

        return Ok(new { message = $"Student with ID {id} has been deleted successfully." });
    }
}
