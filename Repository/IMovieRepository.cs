using MyApp.Helpers.QueryParameters;
using MyApp.Models;

namespace MyApp.Repository;

public interface IMovieRepository
{
    IQueryable<Movie> GetMoviesAsync(MovieQueryParameters parameters, CancellationToken ct);
    Task<Movie?> GetMovieByIdAsync(int id, CancellationToken ct);


    Task AddMovieAsync(Movie movie, CancellationToken ct);

    Task<Movie?> GetByIdForUpdateAsync(int id, CancellationToken ct);
    void RemoveMovieActors(ICollection<MovieActor> movieActors);

    Task SaveChangesAsync(CancellationToken ct);
}
