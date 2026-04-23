using MediatR;
using MyApp.DTOs.Movie;
using MyApp.Validators;

namespace MyApp.Features.Movies.Commands;

public record CreateMovieCommand : IRequest<MovieDetailDto>
{
    public required string Title { get; init; }
    [SkipSanitization]
    public required string Genre { get; set; }
    public int Year { get; set; }
    public int? DirectorId { get; set; }
    public List<ActorMovieAssigment> Actors { get; set; } = new List<ActorMovieAssigment>();
}


public record ActorMovieAssigment
{
    public int ActorId { get; set; }
    public string? Role { get; set; }
}