using MyApp.DTOs.Movie;

namespace MyApp.Services;

public interface IMovieService
{
    Task<IEnumerable<MovieSummaryDto>> GetAllMovies(CancellationToken ct);
    Task<MovieDetailDto?> GetMovieById(int id, CancellationToken ct);
    Task<MovieDetailDto> CreateMovie(CreateMovieRequest movie, CancellationToken ct);
    Task<bool> UpdateMovie(int id, CreateMovieRequest movie, CancellationToken ct);
    Task<bool> PartialUpdateMovie(int id, UpdateMovieRequest movie, CancellationToken ct);
    Task<bool> DeleteMovie(int id, CancellationToken ct);

}