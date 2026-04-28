// server/Controllers/DebugController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace server.Controllers;

public class DebugController : BaseApiV1Controller
{
    [HttpGet("auth-info")]
    public IActionResult GetAuthInfo()
    {
        return Ok(new
        {
            isAuthenticated = User.Identity?.IsAuthenticated ?? false,
            userName = User.Identity?.Name,
            authType = User.Identity?.AuthenticationType,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [Authorize]
    [HttpGet("secured")]
    public IActionResult Secured()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var name = User.FindFirstValue(ClaimTypes.Name);

        return Ok(new
        {
            message = "Вы авторизованы!",
            userId = userId,
            email = email,
            name = name,
            timestamp = DateTime.UtcNow
        });
    }
}
