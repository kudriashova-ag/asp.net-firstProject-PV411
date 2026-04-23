using AutoMapper;
using MediatR;
using MyApp.DTOs.Movie;
using MyApp.Models;
using MyApp.Repository;

namespace MyApp.Features.Movies.Commands;

public class UpdateMovieHandler(
    
    IMovieRepository _movieRepository)
    : IRequestHandler<UpdateMovieCommand, bool>
{
    
    public async Task<bool> Handle(UpdateMovieCommand request, CancellationToken ct)
    {
        var movieToUpdate = await _movieRepository.GetByIdForUpdateAsync(request.Id, ct);
        if (movieToUpdate == null) return false;

        //_mapper.Map(request.movie, movieToUpdate);

        movieToUpdate.Title = request.movie.Title;
        movieToUpdate.Genre = request.movie.Genre;
        movieToUpdate.Year = request.movie.Year;
        movieToUpdate.DirectorId = request.movie.DirectorId;

        _movieRepository.RemoveMovieActors(movieToUpdate.MovieActors);

        movieToUpdate.MovieActors = request.movie.Actors.Select(a => new MovieActor
        {
            ActorId = a.ActorId,
            Role = a.Role,
            MovieId = request.Id
        }).ToList();

        await _movieRepository.SaveChangesAsync(ct);
        return true;
    }

}