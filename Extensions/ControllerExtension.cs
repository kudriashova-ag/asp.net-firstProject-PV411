using Microsoft.AspNetCore.Mvc;

namespace MyApp.Extensions;

public static class ControllerExtension
{
    public static ActionResult NotFoundProblem(
        this ControllerBase controller,
        string detail,
        string? title = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = title ?? "Resource not found",
            Detail = detail,
            Instance = controller.HttpContext.Request.Path
        };

        return controller.NotFound(problemDetails);
    }

    public static ActionResult BadRequestProblem(
        this ControllerBase controller,
        string detail,
        string? title = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = title ?? "Bad request",
            Detail = detail,
            Instance = controller.HttpContext.Request.Path
        };

        return controller.NotFound(problemDetails);
    }
}