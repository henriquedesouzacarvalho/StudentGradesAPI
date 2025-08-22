using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace StudentGradesAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class VersionController : ControllerBase
{
    [HttpGet("version")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public IActionResult GetVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        return Ok(new { Version = version?.ToString() });
    }
}
