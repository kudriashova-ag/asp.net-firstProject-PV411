using AutoMapper;
using MyApp.Data;
using MyApp.DTOs.Movie;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using AutoMapper.QueryableExtensions;
using MyApp.Helpers.Pagination;
using MyApp.Helpers.QueryParameters;
using MyApp.Repository;


namespace MyApp.Services;

public class MovieService : IMovieService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly IMovieRepository _movieRepository;

    public MovieService(AppDbContext db, IMapper mapper, IMovieRepository movieRepository)
    {
        _db = db;
        _mapper = mapper;
        _movieRepository = movieRepository;
    }

    public async Task<PagedResult<MovieSummaryDto>> GetAllMovies(CancellationToken ct, MovieQueryParameters parameters)
    {
        var query = _movieRepository.GetMoviesAsync(parameters, ct);

        return await query
                .ToPagedResultAsync<Movie, MovieSummaryDto>(
                    parameters.Page,
                    parameters.Size,
                    _mapper.ConfigurationProvider, ct);
    }

    public async Task<MovieDetailDto?> GetMovieById(int id, CancellationToken ct)
    {
        var movie = await _movieRepository.GetMovieByIdAsync(id, ct);
        return movie is null ? null : _mapper.Map<MovieDetailDto>(movie);
    }

    public async Task<MovieDetailDto> CreateMovie(CreateMovieRequest movieRequest, CancellationToken ct)
    {
        var newMovie = new Movie
        {
            Title = movieRequest.Title,
            Genre = movieRequest.Genre,
            Year = movieRequest.Year,
            DirectorId = movieRequest.DirectorId,
            MovieActors = movieRequest.Actors.Select(a => new MovieActor
            {
                ActorId = a.ActorId,
                Role = a.Role
            }).ToList()
        };

        await _movieRepository.AddMovieAsync(newMovie, ct);
        
        return (await GetMovieById(newMovie.Id, ct))!;
    }

    public async Task<bool> UpdateMovie(int id, CreateMovieRequest movieRequest, CancellationToken ct)
    {
        var movieToUpdate = await _movieRepository.GetByIdForUpdateAsync(id, ct);
        if (movieToUpdate == null) return false;

        _mapper.Map(movieRequest, movieToUpdate);

        _movieRepository.RemoveMovieActors(movieToUpdate.MovieActors);

        movieToUpdate.MovieActors = movieRequest.Actors.Select(a => new MovieActor
        {
            ActorId = a.ActorId,
            Role = a.Role,
            MovieId = id
        }).ToList();
        
        await _movieRepository.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> PartialUpdateMovie(int id, UpdateMovieRequest movieRequest, CancellationToken ct)
    {
        var movieToUpdate = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id, ct);
        if (movieToUpdate == null) return false;
        _mapper.Map(movieRequest, movieToUpdate);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteMovie(int id, CancellationToken ct)
    {
        var movie = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id, ct);
        if (movie == null) return false;
        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync(ct);
        return true;
    }


    public async Task<bool> DirectorExists(int? DirectorId, CancellationToken ct)
    {
        return await _db.Directors.AnyAsync(d => d.Id == DirectorId, ct);
    }

}