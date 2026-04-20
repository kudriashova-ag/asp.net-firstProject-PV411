using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Identity;
using MyApp.Features.Auth.Commands.Register;
using MyApp.Services;

namespace MyApp.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await _mediator.Send(command, CancellationToken.None);

        if (result.Succeeded)
            return StatusCode(201, "Успішна реєстрація");

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var token = await _mediator.Send(command, CancellationToken.None);

        if (token == null)
            return Unauthorized(new
            {
                message = "Невірний логін або пароль"
            });

        return Ok(new { token });
    }
}