namespace MyApp.Services;

public class FileService
{
    private readonly IWebHostEnvironment _env;
    private readonly string _uploadsRoot;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
        _uploadsRoot = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(_uploadsRoot);
    }


    public async Task<string> SaveFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Файл не передано");

        // Валідація розширення
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowed.Contains(ext))
        {
            throw new ArgumentException($"Недопустимий формат: {ext}");
        }

        //  Перевірка MIME-type
        var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        var mimeType = file.ContentType.ToLowerInvariant();
        if (!allowedMimeTypes.Contains(mimeType))
        {
            throw new ArgumentException(
                $"Недопустимий тип вмісту: {mimeType}." +
                $"Дозволені типи: {string.Join(", ", allowedMimeTypes)}");
        }

        // Структура папок uploads/2026.03.23
        var datePath = DateTime.UtcNow.ToString("yyyy/MM/dd");
        Console.WriteLine(datePath);
        var targetDir = Path.Combine(_uploadsRoot, datePath);
        Directory.CreateDirectory(targetDir);

        var safeFileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(targetDir, safeFileName);

        // Створення та збереження файлу
        await using (var stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        var url = $"/uploads/{datePath}/{safeFileName}";

        return url;
    }

}