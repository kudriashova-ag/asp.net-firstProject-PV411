using System.Diagnostics;
using MediatR;
namespace MyApp.Behaviours;

// Клас є generic — працює з БУДЬ-якою командою/query автоматично.
// TRequest — тип запиту (наприклад CreateOrderCommand)
// TResponse — тип відповіді (наприклад OrderDto)
public class LoggingBehavior<TRequest, TResponse>

    // Реалізує інтерфейс MediatR пайплайну.
    // Саме це дозволяє MediatR автоматично викликати цей клас
    // перед/після кожного handler-а
    : IPipelineBehavior<TRequest, TResponse>

    // Обмеження: TRequest обов'язково має бути MediatR-запитом,
    // що повертає TResponse. Без цього клас не скомпілюється
    where TRequest : IRequest<TResponse>
{
    // Стандартний ILogger з ASP.NET DI.
    // Generic параметр <LoggingBehavior<TRequest, TResponse>> задає
    // категорію логу — саме це ім'я побачиш у Seq/консолі
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    // Конструктор — ASP.NET сам підставить ILogger через DI
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    // Головний метод — MediatR викликає його замість handler-а.
    // Ти сам вирішуєш: що зробити ДО, викликати handler, що зробити ПІСЛЯ
    public async Task<TResponse> Handle(
        TRequest request,          // сам об'єкт запиту з усіма полями
        RequestHandlerDelegate<TResponse> next, // делегат = виклик наступного кроку в пайплайні (або самого handler-а)
        CancellationToken cancellationToken)    // для скасування async операції
    {
        // Витягуємо людське ім'я типу запиту.
        // Замість "CreateOrderCommand`1" отримаємо просто "CreateOrderCommand"
        var requestName = typeof(TRequest).Name;
        if(requestName!= "RegisterCommand")
        // Логуємо початок обробки.
        // {@Request} — оператор @ означає серіалізувати весь об'єкт як JSON,
        // а не просто викликати .ToString(). Дуже зручно в Seq/ELK
        _logger.LogInformation("→ Handling {RequestName} {@Request}",
            requestName, request);

        // Запускаємо таймер щоб виміряти час виконання handler-а
        var stopwatch = Stopwatch.StartNew();
        try
        {
            // ← ОЦЕ НАЙВАЖЛИВІШИЙ РЯДОК
            // Викликає наступний крок пайплайну або сам handler.
            // Якщо не викликати next() — handler НІКОЛИ не виконається!
            var response = await next();

            // Зупиняємо таймер одразу після виконання
            stopwatch.Stop();

            // Логуємо успішне завершення з часом виконання.
            // ElapsedMilliseconds — скільки мілісекунд зайняв handler
            _logger.LogInformation("✓ {RequestName} handled in {ElapsedMs}ms",
                requestName, stopwatch.ElapsedMilliseconds);

            // Повертаємо результат далі — контролеру.
            // Якщо не повернути — відповідь загубиться!
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Логуємо помилку з повним stack trace.
            // Перший параметр ex — це саме він додає stack trace до логу.
            // Без нього побачиш тільки повідомлення без деталей
            _logger.LogError(ex, "✗ {RequestName} failed after {ElapsedMs}ms",
                requestName, stopwatch.ElapsedMilliseconds);

            // Перекидаємо виняток далі — НЕ ковтаємо його.
            // Middleware або ExceptionHandler вище вирішить що з ним робити
            throw;
        }
    }
}