using Microsoft.AspNetCore.Mvc;
using FileAnalysisService.Services;

namespace FileAnalysisService;

[ApiController]
[Route("analyze")]// Базовый маршрут для контроллера анализа
public class InternalAnalysisController : ControllerBase
{
    private readonly IAnalysisService _service;

    public InternalAnalysisController(IAnalysisService service)
    {
        _service = service;
    }

    [HttpGet("{fileId:guid}")] // Маршрут для анализа файла по его ID
    public IActionResult Analyze(Guid fileId)
    {
        var result = _service.WordAnalysis(fileId);
        return result == null ? NotFound() : Ok(result);
    }
}