using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyApp.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;
    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            var validator = _serviceProvider
                .GetService(typeof(IValidator<>)
                .MakeGenericType(argument.GetType())) as IValidator;

            if (validator == null) continue;

            var result = await validator
                .ValidateAsync(new ValidationContext<object>(argument));

            if (!result.IsValid)
            {
                var errors = result.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                context.Result = new BadRequestObjectResult(
                    new ValidationProblemDetails(errors)
                    {
                        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1%22",
                        Title = "One or more validation errors occurred",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = "Please refer to the errors property for additional details"
                    });
                return;
            }
        }
        await next();
    }
}
