// server/Controllers/ApiV1Controller.cs
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

public class ApiV1RouteAttribute : RouteAttribute
{
    public ApiV1RouteAttribute(string template) : base($"api/v1/{template}")
    {
    }
}

