using System.Text.Json;                          // Для серіалізації об'єкта ProblemDetails у JSON
using Microsoft.AspNetCore.Mvc;                  // Потрібно для класу ProblemDetails

namespace MyApp.Middleware;

/* 
Коротке резюме того, що робить цей middleware:

Ловить всі необроблені винятки, які виникають у будь-якій частині додатка.
Перетворює їх у стандартизований формат ProblemDetails.
Повертає правильний HTTP-статус (400, 404 або 500).
У режимі Development додає traceId і stackTrace для зручної діагностики.
У режимі Production повертає мінімальну інформацію (без стеку), щоб не розкривати внутрішню структуру додатка.

 */
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;      // Делегат, який вказує на наступний middleware у пайплайні

    // Конструктор. ASP.NET Core автоматично передає сюди наступний middleware
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Головний метод middleware. Викликається для кожного HTTP-запиту.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Передаємо управління наступному middleware (або кінцевій точці — контролеру)
            await _next(context);
        }
        catch (Exception ex)                       // Якщо будь-де нижче по ланцюжку стався виняток — ловимо його тут
        {
            // Обробляємо помилку і формуємо відповідь клієнту
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Приватний метод, який формує стандартизовану відповідь у форматі ProblemDetails
    /// </summary>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Встановлюємо правильний Content-Type згідно зі стандартом RFC 7807
        context.Response.ContentType = "application/problem+json";

        // Визначаємо HTTP-статус код залежно від типу винятку
        context.Response.StatusCode = exception switch
        {
            // Бізнес-логічні помилки та помилки аргументів → 400 Bad Request
            ArgumentException or InvalidOperationException => StatusCodes.Status400BadRequest,

            // Коли сутність не знайдена (наприклад, фільм за Id) → 404 Not Found
            KeyNotFoundException => StatusCodes.Status404NotFound,

            // Усі інші непередбачені помилки → 500 Internal Server Error
            _ => StatusCodes.Status500InternalServerError
        };

        // Створюємо стандартний об'єкт ProblemDetails
        var problemDetails = new ProblemDetails
        {
            // Статус код, який ми встановили вище
            Status = context.Response.StatusCode,

            // Заголовок помилки (Title) — короткий опис проблеми
            Title = exception switch
            {
                ArgumentException => "Bad Request",
                KeyNotFoundException => "Not Found",
                _ => "Internal Server Error"
            },

            // Детальний опис помилки — повідомлення з винятку
            Detail = exception.Message,

            // Шлях, на якому сталася помилка (наприклад: /api/v1/movie/999)
            Instance = context.Request.Path
        };

        // Серіалізуємо ProblemDetails у JSON з camelCase (щоб відповідало стандартам JavaScript)
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Записуємо JSON-відповідь у тіло HTTP-відповіді
        await context.Response.WriteAsync(json);
    }
}