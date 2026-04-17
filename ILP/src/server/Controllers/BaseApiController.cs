using Microsoft.AspNetCore.Mvc;
using server.Data.DTOs;
using server.Services;

namespace server.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : Controller
{

}


public abstract class BaseApiV1Controller : BaseApiController
{

}
