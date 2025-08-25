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
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                     ?? assembly.GetName().Version?.ToString();

        return Ok(new { Version = version?.ToString() });
    }
}
