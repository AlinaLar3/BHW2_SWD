using Microsoft.AspNetCore.Mvc;
using FileAnalysisService.Services;

namespace FileAnalysisService;

[ApiController]
[Route("analyze")]// ������� ������� ��� ����������� �������
public class InternalAnalysisController : ControllerBase
{
    private readonly IAnalysisService _service;

    public InternalAnalysisController(IAnalysisService service)
    {
        _service = service;
    }

    [HttpGet("{fileId:guid}")] // ������� ��� ������� ����� �� ��� ID
    public IActionResult Analyze(Guid fileId)
    {
        var result = _service.WordAnalysis(fileId);
        return result == null ? NotFound() : Ok(result);
    }
}