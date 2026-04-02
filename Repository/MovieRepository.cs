using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Helpers.Queryable;
using MyApp.Helpers.QueryParameters;
using MyApp.Models;

namespace MyApp.Repository;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _db;

    public MovieRepository(AppDbContext db)
    {
        _db = db;
    }

    public IQueryable<Movie> GetMoviesAsync(MovieQueryParameters parameters, CancellationToken ct)
    {
        return _db.Movies
                .AsNoTracking()
                .ApplyFilters(parameters)
                .ApplySorting(parameters.Sort);
    }

    public async Task<Movie?> GetMovieByIdAsync(int id, CancellationToken ct)
    {

        return await _db.Movies.
                        AsNoTracking()
                        .Where(m => m.Id == id)
                        .Include(m => m.Director)
                        .Include(m => m.MovieActors)
                            .ThenInclude(ma => ma.Actor)
                        .FirstOrDefaultAsync(ct);
    }

    public async Task AddMovieAsync(Movie movie, CancellationToken ct)
    {
        await _db.Movies.AddAsync(movie, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Movie?> GetByIdForUpdateAsync(int id, CancellationToken ct)
    {
        return await _db.Movies
                        .Where(m => m.Id == id)
                        .Include(m => m.MovieActors)
                        .FirstOrDefaultAsync(ct);
    }

    public Task RemoveMovieActors(ICollection<MovieActor> movieActors)
    {
        _db.RemoveRange(movieActors);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _db.SaveChangesAsync(ct);
    }

    public Task RemoveMovie(Movie movie, CancellationToken ct)
    {
        _db.Movies.Remove(movie);
        return Task.CompletedTask;
    }


    public async Task<bool> DirectorExistsAwait(int? DirectorId, CancellationToken ct)
    {
        return await _db.Directors.AnyAsync(d => d.Id == DirectorId, ct);
    }
}