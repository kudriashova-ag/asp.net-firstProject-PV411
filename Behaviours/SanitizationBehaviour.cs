using Ganss.Xss;
using MediatR;
using MyApp.Validators;
using System.Reflection;

namespace MyApp.Behaviours;


public class SanitizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHtmlSanitizer _sanitizer;

    public SanitizationBehaviour(IHtmlSanitizer sanitizer)
    {
        _sanitizer = sanitizer;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        SanitizeObject(request);
        return await next();
    }

    private void SanitizeObject(object? obj)
    {
        if (obj is null) return;

        var type = obj.GetType();

        // Пропускаємо примітиви та системні типи
        if (type.IsPrimitive || type.Namespace?.StartsWith("System") == true && type != typeof(string))
            return;

        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

        foreach (var prop in properties)
        {
            try
            {
                if (prop.PropertyType == typeof(string))
                {
                    if (prop.GetCustomAttribute<SkipSanitizationAttribute>() is not null)
                        continue;
                        
                    var value = prop.GetValue(obj) as string;
                    if (!string.IsNullOrEmpty(value))
                    {
                        var sanitized = _sanitizer.Sanitize(value);
                        SetValue(obj, prop, sanitized);
                    }
                }
                else if (prop.PropertyType.IsClass && !prop.PropertyType.IsEnum)
                {
                    var nestedValue = prop.GetValue(obj);
                    if (nestedValue is null) continue;

                    // Обробка колекцій (List<T>, IEnumerable<T>)
                    if (nestedValue is System.Collections.IEnumerable enumerable and not string)
                    {
                        foreach (var item in enumerable)
                            SanitizeObject(item);
                    }
                    else
                    {
                        SanitizeObject(nestedValue);
                    }
                }
            }
            catch (TargetInvocationException) { }
        }
    }

    private static void SetValue(object obj, PropertyInfo prop, string sanitized)
    {
        // Спроба через звичайний setter
        if (prop.CanWrite)
        {
            prop.SetValue(obj, sanitized);
            return;
        }

        // Для init-only (record) — через backing field
        var backingField = obj.GetType().GetField(
            $"<{prop.Name}>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Instance);

        backingField?.SetValue(obj, sanitized);
    }
}