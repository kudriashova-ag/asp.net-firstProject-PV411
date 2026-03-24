using Microsoft.AspNetCore.Mvc;
using MyApp.Services;

namespace MyApp.Controllers.V1;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly FileService _fileService;
    public FilesController(FileService fileService)
    {
        _fileService = fileService;
    }


    [HttpPost("upload")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            var url = await _fileService.SaveFile(file);
            return Ok(url);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (IOException ex)
        {
            return StatusCode(500, "Помилка сервера при збереженні файлу " + ex.Message);
        };
       
    }
}