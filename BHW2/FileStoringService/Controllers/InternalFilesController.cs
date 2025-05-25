
using FileStoringService.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileStoringService;

[ApiController]
[Route("files")] // Базовый маршрут контроллера, разрешается в /InternalFiles
public class InternalFilesController : ControllerBase
{
    private readonly IFileStorageService _fileService;

    public InternalFilesController(IFileStorageService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet("download/{id:guid}")] // Маршрут для скачивания файла по его ID
    public IActionResult Download(Guid id)
    {
        try
        {
            var (stream, contentType, fileName) = _fileService.RetrieveFile(id);
            var result = new FileStreamResult(stream, contentType)
            {
                FileDownloadName = fileName
            };
            return result;
        }
        catch (FileNotFoundException)
        {
            return NotFound("File not found");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving file: {ex.Message}");
            return StatusCode(500, "Error retrieving file");
        }
    }

    [HttpPost("upload")] // Маршрут для загрузки нового файла
    [Consumes("multipart/form-data")]
    public IActionResult UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File not uploaded");
        }
        try
        {
            var id = _fileService.StoreFile(file);

            return Ok(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading file: {ex.Message}");
            return StatusCode(500, "Error saving file");
        }
    }
}
