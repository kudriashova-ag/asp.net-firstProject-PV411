using MediatR;
using Microsoft.AspNetCore.Identity;
using MyApp.Models;
using Services;

namespace MyApp.Features.Auth.Commands.Register;


public class LoginCommandHandler(
    UserManager<ApplicationUser> _userManager,
    TokenService _tokenService,
    ILogger<LoginCommandHandler> _logger
    ) : IRequestHandler<LoginCommand, string?>
{

    public async Task<string?> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return null;

        _logger.LogInformation($"User {user.UserName} with ID {user.Id} logged in");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return _tokenService.GenerateToken(user, roles);
    }
}