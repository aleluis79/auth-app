using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth_app.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet("ping")]
    [Authorize]
    public ActionResult<string> Ping()
    {
        _logger.LogInformation("Ejecutando método ping");
        return Ok("pong");
    }
}
