using MediatR;

namespace MyApp.Features.Movies.Commands;

public record UpdateMovieCommand(int Id, CreateMovieCommand movie) : IRequest<bool>;
