namespace MyApp.Services;

public interface IFileService
{
    Task<string> SaveFile(IFormFile file);
}
