using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApiGateway;

[ApiController]
[Route("files")] // Базовый маршрут для файловых операций
public class FilesController : ControllerBase
{
    private const string FileStorageBaseUrl = "http://file-storage:8001";

    private readonly IHttpClientFactory _httpClientFactory;

    public FilesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("upload")] // Маршрут для загрузки файла
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var client = _httpClientFactory.CreateClient();
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(memoryStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
        content.Add(fileContent, "file", file.FileName); 

        // Перенаправление запроса в FileStoringService
        var response = await client.PostAsync($"{FileStorageBaseUrl}/files/upload", content);
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, $"Error from file-storage: {errorMessage}");
        }

        var result = await response.Content.ReadAsStringAsync();
        return Content(result, "application/json");
    }

    [HttpGet("{fileId:guid}")] // Маршрут для скачивания файла по ID
    public async Task<IActionResult> GetFile(Guid fileId)
    {
        var client = _httpClientFactory.CreateClient();
        // Перенаправление запроса в FileStoringService
        var file = await client.GetAsync($"{FileStorageBaseUrl}/files/download/{fileId}");
        if (!file.IsSuccessStatusCode)
        {
            var err = await file.Content.ReadAsStringAsync();
            return StatusCode((int)file.StatusCode, $"Error from file-storage: {err}");
        }

        var stream = await file.Content.ReadAsStreamAsync();
        var contentType = file.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
        var fileName = file.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? "downloaded_file";

        return File(stream, contentType, fileDownloadName: fileName); 
    }
}

