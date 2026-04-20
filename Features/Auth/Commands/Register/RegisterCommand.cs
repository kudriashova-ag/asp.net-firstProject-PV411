using MediatR;
using Microsoft.AspNetCore.Identity;

namespace MyApp.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string ConfirmPassword) : IRequest<IdentityResult>;