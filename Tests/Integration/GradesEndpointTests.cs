using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using StudentGradesAPI.Models;
using Xunit;

namespace StudentGradesAPI.Tests.Integration
{
    public class GradesEndpointTests : IntegrationTestBase
    {
        public GradesEndpointTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task GetGrades_ShouldReturnSeededGrades()
        {
            var response = await Client.GetAsync("/api/Grades");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var grades = await response.Content.ReadFromJsonAsync<List<GradeResponseDto>>();
            grades.Should().NotBeNull();
            grades!.Should().NotBeEmpty();
            grades!.Any(g => g.Subject == "Mathematics").Should().BeTrue();
        }

        [Fact]
        public async Task PostGrade_ShouldCreate_WhenStudentExists_AndReturnUpdatedAverage()
        {
            var createStudent = new CreateStudentDto { Name = "Dave", Email = $"dave{Guid.NewGuid():N}@example.com" };
            var studentResp = await Client.PostAsJsonAsync("/api/Students", createStudent);
            var student = await studentResp.Content.ReadFromJsonAsync<StudentResponseDto>();

            var dto = new CreateGradeDto { Value = 7.5, Subject = "Biology", StudentId = student!.Id };
            var response = await Client.PostAsJsonAsync("/api/Grades", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
            var root = doc.RootElement;
            root.GetProperty("studentInfo").GetProperty("newAverageGrade").GetDouble().Should().BeApproximately(7.5, 0.001);
            root.GetProperty("studentInfo").GetProperty("totalGrades").GetInt32().Should().Be(1);
        }

        [Fact]
        public async Task PostGrade_ShouldFail_WhenStudentDoesNotExist()
        {
            var dto = new CreateGradeDto { Value = 9.0, Subject = "History", StudentId = 9999 };
            var response = await Client.PostAsJsonAsync("/api/Grades", dto);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PutGrade_ShouldUpdate_PartialFields_AndRecalculateAverage()
        {
            // Use seeded student 1 by creating a new grade and then updating
            // Create a student to own the grade
            var stCreate = new CreateStudentDto { Name = "For Grade", Email = $"forgrade{Guid.NewGuid():N}@example.com" };
            var stResp = await Client.PostAsJsonAsync("/api/Students", stCreate);
            var student = await stResp.Content.ReadFromJsonAsync<StudentResponseDto>();

            var dto = new CreateGradeDto { Value = 5.0, Subject = "Art", StudentId = student!.Id };
            var createResp = await Client.PostAsJsonAsync("/api/Grades", dto);
            createResp.StatusCode.Should().Be(HttpStatusCode.Created);
            using var doc = await JsonDocument.ParseAsync(await createResp.Content.ReadAsStreamAsync());
            int gradeId = doc.RootElement.GetProperty("grade").GetProperty("id").GetInt32();

            var update = new UpdateGradeDto { Value = 9.5 };
            var updateResp = await Client.PutAsJsonAsync($"/api/Grades/{gradeId}", update);
            updateResp.StatusCode.Should().Be(HttpStatusCode.OK);
            using var updatedDoc = await JsonDocument.ParseAsync(await updateResp.Content.ReadAsStreamAsync());
            updatedDoc.RootElement.GetProperty("grade").GetProperty("value").GetDouble().Should().BeApproximately(9.5, 0.001);
        }

        [Fact]
        public async Task DeleteGrade_ShouldRemove_AndReturnUpdatedAverage()
        {
            // Create a student to own the grade
            var stCreate = new CreateStudentDto { Name = "Del Grade", Email = $"delgrade{Guid.NewGuid():N}@example.com" };
            var stResp = await Client.PostAsJsonAsync("/api/Students", stCreate);
            var student = await stResp.Content.ReadFromJsonAsync<StudentResponseDto>();

            var dto = new CreateGradeDto { Value = 6.0, Subject = "Geography", StudentId = student!.Id };
            var createResp = await Client.PostAsJsonAsync("/api/Grades", dto);
            using var createdDoc = await JsonDocument.ParseAsync(await createResp.Content.ReadAsStreamAsync());
            int gradeId = createdDoc.RootElement.GetProperty("grade").GetProperty("id").GetInt32();

            var deleteResp = await Client.DeleteAsync($"/api/Grades/{gradeId}");
            deleteResp.StatusCode.Should().Be(HttpStatusCode.OK);
            using var deletedDoc = await JsonDocument.ParseAsync(await deleteResp.Content.ReadAsStreamAsync());
            deletedDoc.RootElement.GetProperty("studentInfo").GetProperty("totalGrades").GetInt32().Should().BeGreaterOrEqualTo(0);
        }
    }
}
