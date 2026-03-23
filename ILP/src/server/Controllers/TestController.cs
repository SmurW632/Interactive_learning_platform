using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using server.Models;

namespace server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("connection")]
    public IActionResult Connection()
    {
        return Ok(new { message = "Сервер работает!", time = DateTime.Now });
    }

}
