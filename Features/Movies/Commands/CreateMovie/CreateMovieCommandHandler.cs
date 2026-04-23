using AutoMapper;
using MediatR;
using MyApp.DTOs.Movie;
using MyApp.Models;
using MyApp.Repository;

namespace MyApp.Features.Movies.Commands;

public class CreateMovieHandler(
    IMapper _mapper,
    IMovieRepository _movieRepository)
    : IRequestHandler<CreateMovieCommand, MovieDetailDto>
{
    
    public async Task<MovieDetailDto> Handle(CreateMovieCommand request, CancellationToken ct)
    {
        var newMovie = new Movie
        {
            Title = request.Title,
            Genre = request.Genre,
            Year = request.Year,
            DirectorId = request.DirectorId,
            MovieActors = request.Actors.Select(a => new MovieActor
            {
                ActorId = a.ActorId,
                Role = a.Role
            }).ToList()
        };

        await _movieRepository.AddMovieAsync(newMovie, ct);
        var created = await _movieRepository.GetMovieByIdAsync(newMovie.Id, ct);
        
        return _mapper.Map<MovieDetailDto>(created);
    }

}