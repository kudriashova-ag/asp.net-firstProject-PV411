using MediatR;
using Microsoft.AspNetCore.Identity;
using MyApp.Models;

namespace MyApp.Features.Auth.Commands.Register;


public class RegisterCommandHandler: IRequestHandler<RegisterCommand, IdentityResult>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterCommandHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(RegisterCommand request, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        return await _userManager.CreateAsync(user, request.Password);
    }
}