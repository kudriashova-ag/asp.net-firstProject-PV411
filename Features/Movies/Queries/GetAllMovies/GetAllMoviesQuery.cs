using MediatR;
using MyApp.DTOs.Movie;
using MyApp.Helpers.Pagination;
using MyApp.Helpers.QueryParameters;

namespace MyApp.Features.Movies.Queries.GetAllMovies;

public record GetAllMoviesQuery(MovieQueryParameters Parameters) : IRequest<PagedResult<MovieSummaryDto>>;
