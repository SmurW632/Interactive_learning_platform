using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Services.PythonResearch;

namespace server.Controllers;

public class ResearchController : BaseApiV1Controller
{
    private readonly IPythonResearchService _pythonResearch;

    public ResearchController(IPythonResearchService pythonResearch)
    {
        _pythonResearch = pythonResearch;
    }

    public record ResearchRequest(string Query);
    public record ResearchResponse(string Result);

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] ResearchRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest("Query is required");

        var result = await _pythonResearch.ResearchAsync(request.Query, ct);
        return Ok(new ResearchResponse(result));
    }
}
