using AutoMapper;
using MediatR;
using MyApp.DTOs.Movie;
using MyApp.Helpers.Pagination;
using MyApp.Models;
using MyApp.Repository;

namespace MyApp.Features.Movies.Queries.GetAllMovies;

public class GetAllMoviesQueryHandler : IRequestHandler<GetAllMoviesQuery, PagedResult<MovieSummaryDto>>
{
    private readonly IMapper _mapper;
    private readonly IMovieRepository _movieRepository;

    public GetAllMoviesQueryHandler(IMapper mapper, IMovieRepository movieRepository)
    {
        _mapper = mapper;
        _movieRepository = movieRepository;
    }

    public async Task<PagedResult<MovieSummaryDto>> Handle(GetAllMoviesQuery request, CancellationToken cancellationToken)
    {
        var query = _movieRepository.GetMoviesAsync(request.Parameters, cancellationToken);

        return await query
                .ToPagedResultAsync<Movie, MovieSummaryDto>(
                    request.Parameters.Page,
                    request.Parameters.Size,
                    _mapper.ConfigurationProvider, cancellationToken);
    }

}