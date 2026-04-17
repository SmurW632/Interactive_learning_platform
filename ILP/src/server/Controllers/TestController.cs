using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models;

namespace server.Controllers;

[Authorize]
public class TestController : BaseApiV1Controller
{
    [HttpGet("connection")]
    public IActionResult Connection()
    {
        return Ok(new { message = "Сервер работает!", time = DateTime.Now });
    }

}
