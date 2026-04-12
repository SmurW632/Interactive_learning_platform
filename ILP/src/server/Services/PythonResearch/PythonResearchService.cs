using System.Net.Http.Json;

namespace server.Services.PythonResearch;

public class PythonResearchRequest
{
    public string Query { get; set; } = string.Empty;
}

public class PythonResearchResponse
{
    public string Result { get; set; } = string.Empty;
}

public interface IPythonResearchService
{
    Task<string> ResearchAsync(string query, CancellationToken ct = default);
}

public class PythonResearchService : IPythonResearchService
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public PythonResearchService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<string> ResearchAsync(string query, CancellationToken ct = default)
    {
        var url = _config["ExternalServices:PythonResearchUrl"]
                  ?? throw new InvalidOperationException("PythonResearchUrl not configured");

        var request = new PythonResearchRequest { Query = query };

        var response = await _http.PostAsJsonAsync(url, request, ct);
        var body = await response.Content.ReadFromJsonAsync<PythonResearchResponse>(cancellationToken: ct);

        if (!response.IsSuccessStatusCode || body is null)
            throw new Exception($"Python service error: {(int)response.StatusCode} {response.ReasonPhrase}");

        return body.Result;
    }
}
