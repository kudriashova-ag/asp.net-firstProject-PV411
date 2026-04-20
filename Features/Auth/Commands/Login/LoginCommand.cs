using MediatR;

namespace MyApp.Features.Auth.Commands.Register;

public record LoginCommand(string Email, string Password) : IRequest<string?>;