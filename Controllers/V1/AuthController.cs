using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MyApp.DTOs.Identity;
using MyApp.Services;

namespace MyApp.Controllers.V1;

[ApiVersion(1.0)]
[ApiController]
[Produces("application/json")]
[Route("api/v{version:apiVersion}/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);

        if (result.Succeeded)
            return Ok(new { message = "Реєстрація прошла успішно" });

        return BadRequest(new
        {
            message = "Помилка реєстрації",
            errors = result.Errors
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);

        if (token == null)
            return Unauthorized(new
            {
                message = "Невірний логін або пароль"
            });


        return Ok(new { token });
    }
}