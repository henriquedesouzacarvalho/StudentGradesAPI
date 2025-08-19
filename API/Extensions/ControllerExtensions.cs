using Microsoft.AspNetCore.Mvc;

namespace StudentGradesAPI.Extensions;

public static class ControllerExtensions
{
    public static NotFoundObjectResult EntityNotFound(this ControllerBase controller, string entityName, int id)
    {
        return controller.NotFound(new { message = $"{entityName} with ID {id} not found." });
    }
}