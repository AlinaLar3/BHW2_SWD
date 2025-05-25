using Microsoft.AspNetCore.Mvc;

namespace ApiGateway;

[ApiController]
[Route("analyze")] // Базовый маршрут для запросов анализа
public class AnalysisController : ControllerBase
{
    private const string AnalysBaseUrl = "http://file-analysis:8002";
    private readonly IHttpClientFactory _httpClientFactory;

    public AnalysisController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("{fileId:guid}")]
    public async Task<IActionResult> Analyze(Guid fileId)
    {
        var client = _httpClientFactory.CreateClient();
        // Перенаправление запроса на FileAnalysisService
        var response = await client.GetAsync($"{AnalysBaseUrl}/analyze/{fileId}");
        var result = await response.Content.ReadAsStringAsync();

        return Content(result, "application/json");
    }
}
