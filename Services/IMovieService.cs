using MyApp.DTOs.Movie;
using MyApp.Helpers.Pagination;
using MyApp.Helpers.QueryParameters;

namespace MyApp.Services;

public interface IMovieService
{
    Task<PagedResult<MovieSummaryDto>> GetAllMovies(CancellationToken ct, MovieQueryParameters parameters);
    Task<MovieDetailDto?> GetMovieById(int id, CancellationToken ct);
    Task<MovieDetailDto> CreateMovie(CreateMovieRequest movie, CancellationToken ct);
    Task<bool> UpdateMovie(int id, CreateMovieRequest movie, CancellationToken ct);
    Task<bool> PartialUpdateMovie(int id, UpdateMovieRequest movie, CancellationToken ct);
    Task<bool> DeleteMovie(int id, CancellationToken ct);
    Task<bool> DirectorExists(int? DirectorId, CancellationToken ct);

}