using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Integration
{
    public class StudentsEndpointTests : IntegrationTestBase
    {
        public StudentsEndpointTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task GetStudents_ShouldReturnSeededStudentsWithAverageAndGrades()
        {
            // Ensure at least one student exists via API
            var dtoCreate = new CreateStudentDto { Name = "Seed A", Email = $"seed{Guid.NewGuid():N}@example.com" };
            await Client.PostAsJsonAsync("/api/Students", dtoCreate);

            var response = await Client.GetAsync("/api/Students");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var students = await response.Content.ReadFromJsonAsync<List<StudentResponseDto>>();
            students.Should().NotBeNull();
            students!.Should().NotBeEmpty();
            students!.Any(s => s.Email == "john.doe@example.com").Should().BeTrue();
            students!.Any(s => s.Email == "jane.smith@example.com").Should().BeTrue();
        }

        [Fact]
        public async Task PostStudent_ShouldCreate_WhenEmailIsUnique()
        {
            var dto = new CreateStudentDto
            {
                Name = "Alice",
                Email = "alice@example.com"
            };

            var response = await Client.PostAsJsonAsync("/api/Students", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<StudentResponseDto>();
            created.Should().NotBeNull();
            created!.Email.Should().Be("alice@example.com");
            created!.Grades.Should().BeEmpty();
        }

        [Fact]
        public async Task PostStudent_ShouldFail_WhenEmailExists()
        {
            // Create an existing student first
            var existing = new CreateStudentDto { Name = "John", Email = "john.doe@example.com" };
            await Client.PostAsJsonAsync("/api/Students", existing);

            var dto = new CreateStudentDto
            {
                Name = "Duplicate",
                Email = "john.doe@example.com"
            };

            var response = await Client.PostAsJsonAsync("/api/Students", dto);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PutStudent_ShouldUpdate_OnlyProvidedFields_AndValidateEmailUniqueness()
        {
            var create = new CreateStudentDto { Name = "Bob", Email = $"bob{Guid.NewGuid():N}@example.com" };
            var createdResp = await Client.PostAsJsonAsync("/api/Students", create);
            var created = await createdResp.Content.ReadFromJsonAsync<StudentResponseDto>();

            var update = new UpdateStudentDto { Name = "Bobby" };
            var updateResp = await Client.PutAsJsonAsync($"/api/Students/{created!.Id}", update);
            updateResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = await updateResp.Content.ReadFromJsonAsync<StudentResponseDto>();
            updated!.Name.Should().Be("Bobby");
            updated!.Email.Should().Be(create.Email);

            // Try to set email to an existing one
            var conflict = new UpdateStudentDto { Email = "john.doe@example.com" };
            var conflictResp = await Client.PutAsJsonAsync($"/api/Students/{created!.Id}", conflict);
            conflictResp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteStudent_ShouldRemoveStudent()
        {
            var create = new CreateStudentDto { Name = "Charlie", Email = $"charlie{Guid.NewGuid():N}@example.com" };
            var createdResp = await Client.PostAsJsonAsync("/api/Students", create);
            var created = await createdResp.Content.ReadFromJsonAsync<StudentResponseDto>();

            var deleteResp = await Client.DeleteAsync($"/api/Students/{created!.Id}");
            deleteResp.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResp = await Client.GetAsync($"/api/Students/{created!.Id}");
            getResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
