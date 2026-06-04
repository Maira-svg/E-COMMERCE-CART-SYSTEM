using Microsoft.AspNetCore.Mvc;

namespace DarazUltimate.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { 
            status = "Healthy", 
            timestamp = DateTime.UtcNow,
            application = "Daraz Ultimate"
        });
    }
}
