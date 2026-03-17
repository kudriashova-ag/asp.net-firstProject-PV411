using AutoMapper;
using MyApp.Data;
using MyApp.DTOs.Movie;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;


namespace MyApp.Services;

public class MovieService : IMovieService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public MovieService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MovieSummaryDto>> GetAllMovies(CancellationToken ct)
    {
        var movies = await _db.Movies.AsNoTracking().ToListAsync(ct);
        var movieSummary = _mapper.Map<IEnumerable<MovieSummaryDto>>(movies);

        return movieSummary;
    }

    public async Task<MovieDetailDto> GetMovieById(int id, CancellationToken ct)
    {
        var movie = await _db.Movies
                .AsNoTracking()
                .Include(m => m.Director)
                .FirstOrDefaultAsync(m => m.Id == id, ct);

        var movieDetail = _mapper.Map<MovieDetailDto>(movie);
        return movieDetail;
    }

    public async Task<MovieDetailDto> CreateMovie(CreateMovieRequest movieRequest, CancellationToken ct)
    {
        var newMovie = _mapper.Map<Movie>(movieRequest);
        _db.Movies.Add(newMovie);
        await _db.SaveChangesAsync(ct);
        var movieDetail = _mapper.Map<MovieDetailDto>(newMovie);
        return movieDetail;
    }

    public async Task<bool> UpdateMovie(int id, CreateMovieRequest movieRequest, CancellationToken ct)
    {
        var movieToUpdate = await _db.Movies.FirstOrDefaultAsync(m => m.Id == id, ct);
        if (movieToUpdate == null) return false;
        _mapper.Map(movieRequest, movieToUpdate);
        await _db.SaveChangesAsync(ct);
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

}