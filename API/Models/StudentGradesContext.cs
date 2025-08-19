using Microsoft.EntityFrameworkCore;

namespace StudentGradesAPI.Models;

public sealed class StudentGradesContext : DbContext
{
    public StudentGradesContext(DbContextOptions<StudentGradesContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }

    public DbSet<Grade> Grades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Student entity
        _ = modelBuilder.Entity<Student>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            _ = entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            _ = entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Grade entity
        _ = modelBuilder.Entity<Grade>(entity =>
        {
            _ = entity.HasKey(e => e.Id);
            _ = entity.Property(e => e.Value).IsRequired();
            _ = entity.Property(e => e.Subject).IsRequired().HasMaxLength(100);

            // Configure relationship
            _ = entity.HasOne(g => g.Student)
                  .WithMany(s => s.Grades)
                  .HasForeignKey(g => g.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed some initial data
        _ = modelBuilder.Entity<Student>().HasData(
            new Student { Id = 1, Name = "John Doe", Email = "john.doe@example.com", CreatedAt = DateTime.UtcNow },
            new Student { Id = 2, Name = "Jane Smith", Email = "jane.smith@example.com", CreatedAt = DateTime.UtcNow });

        _ = modelBuilder.Entity<Grade>().HasData(
            new Grade { Id = 1, Value = 8.5, Subject = "Mathematics", StudentId = 1, CreatedAt = DateTime.UtcNow },
            new Grade { Id = 2, Value = 9.0, Subject = "Physics", StudentId = 1, CreatedAt = DateTime.UtcNow },
            new Grade { Id = 3, Value = 7.5, Subject = "Chemistry", StudentId = 2, CreatedAt = DateTime.UtcNow });
    }
}
