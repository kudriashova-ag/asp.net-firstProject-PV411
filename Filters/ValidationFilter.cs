using FluentValidation;                    // Підключаємо бібліотеку FluentValidation для роботи з валідаторами
using Microsoft.AspNetCore.Mvc;            // Необхідно для BadRequestObjectResult та ValidationProblemDetails
using Microsoft.AspNetCore.Mvc.Filters;    // Тут знаходиться інтерфейс IAsyncActionFilter

namespace MyApp.Filters;
/* 
Фільтр запускається перед кожним методом контролера.
Він автоматично знаходить потрібний валідатор для вхідної моделі.
Виконує валідацію.
При помилках — зупиняє виконання і повертає гарний ProblemDetails.
При успіху — просто продовжує роботу (await next()).
 */
public class ValidationFilter : IAsyncActionFilter   // Реалізуємо асинхронний Action Filter
{
    private readonly IServiceProvider _serviceProvider;   // Зберігаємо провайдер сервісів, щоб діставати валідатори через DI

    // Конструктор. ASP.NET Core автоматично інжектує IServiceProvider
    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Головний метод фільтра. Виконується перед запуском дії контролера (OnActionExecution).
    /// </summary>
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,      // Контекст виконання дії: містить інформацію про поточний метод, параметри тощо
        ActionExecutionDelegate next)        // Делегат, який дозволяє продовжити виконання наступних фільтрів і самої дії
    {
        // 1. Визначаємо тип валідатора динамічно на основі типу параметра методу контролера
        // Наприклад, якщо метод приймає CreateMovieRequest, то шукаємо IValidator<CreateMovieRequest>
        var validatorType = typeof(IValidator<>)
            .MakeGenericType(context.ActionDescriptor.Parameters[0].ParameterType);

        // 2. Пробуємо отримати екземпляр валідатора з контейнера DI
        var validator = _serviceProvider.GetService(validatorType) as IValidator;

        // 3. Якщо для даного типу моделі валідатор не зареєстрований — просто пропускаємо фільтр
        if (validator != null)
        {
            // 4. Отримуємо сам об'єкт моделі, який прийшов у метод контролера
            // (наприклад, екземпляр CreateMovieRequest)
            var model = context.ActionArguments.Values.FirstOrDefault();

            if (model != null)
            {
                // 5. Виконуємо асинхронну валідацію моделі
                var validationResult = await validator.ValidateAsync(
                    new ValidationContext<object>(model));   // ValidationContext — контекст валідації FluentValidation

                // 6. Якщо валідація НЕ пройшла успішно
                if (!validationResult.IsValid)
                {
                    // 7. Групуємо помилки за назвою властивості (щоб у відповіді був словник)
                    var errors = validationResult.Errors
                        .GroupBy(e => e.PropertyName)                    // Групуємо по імені поля (Title, Year і т.д.)
                        .ToDictionary(
                            g => g.Key,                                   // Ключ — назва властивості
                            g => g.Select(e => e.ErrorMessage).ToArray() // Значення — масив повідомлень про помилки
                        );

                    // 8. Створюємо стандартизований ValidationProblemDetails і повертаємо 400 Bad Request
                    context.Result = new BadRequestObjectResult(new ValidationProblemDetails(errors)
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",   // Стандартне посилання на Bad Request
                        Title = "One or more validation errors occurred",             // Заголовок помилки
                        Status = StatusCodes.Status400BadRequest,                     // HTTP статус 400
                        Detail = "Please refer to the errors property for additional details" // Пояснення
                    });

                    return;   // Важливо! Припиняємо подальше виконання дії контролера
                }
            }
        }

        // 9. Якщо валідатор не знайдено або валідація пройшла успішно — продовжуємо виконання пайплайну
        await next();
    }
}