using AutoMapper;
using MyApp.Data;
using MyApp.DTOs.Movie;
using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using AutoMapper.QueryableExtensions;
using MyApp.DTOs.Director;
using MyApp.DTOs.Actor;
using MyApp.Helpers.Pagination;
using MyApp.Helpers.QueryParameters;


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

    public async Task<PagedResult<MovieSummaryDto>> GetAllMovies(CancellationToken ct, MovieQueryParameters parameters)
    {
        var query = _db.Movies.AsNoTracking().AsQueryable();
        query = ApplyFilters(query, parameters.Search, parameters.Genre, parameters.MinYear, parameters.MaxYear);
        query = ApplySorting(query, parameters.Sort);

        return await query
                .ToPagedResultAsync<Movie, MovieSummaryDto>(
                    parameters.Page,
                    parameters.Size,
                    _mapper.ConfigurationProvider, ct);
    }

    public async Task<MovieDetailDto?> GetMovieById(int id, CancellationToken ct)
    {

        // 1 варіант
        /*   var movie = await _db.Movies
                  .AsNoTracking()
                  .Include(m => m.Director)
                  .FirstOrDefaultAsync(m => m.Id == id, ct);

          var movieDetail = _mapper.Map<MovieDetailDto>(movie); 

          return movieDetail; */

        // 2 варіант
        // return await _db.Movies
        //         .AsNoTracking()
        //         .Where(m => m.Id == id)
        //         .ProjectTo<MovieDetailDto>(_mapper.ConfigurationProvider)
        //         .FirstOrDefaultAsync(ct);

        // 3 варіант
        return await _db.Movies
              .AsNoTracking()
              .Where(m => m.Id == id)
              .Select(m => new MovieDetailDto(
                      m.Id,
                      m.Title,
                      m.Genre,
                      m.Year,
                      m.MovieActors.Select(ma =>
                          new ActorInMovieDto(
                              ma.Actor.Id,
                              ma.Actor.FirstName,
                              ma.Actor.LastName,
                              ma.Role
                          ))
                          .ToList(),
                      m.Director != null ?
                          new DirectorDto(m.Director.Id, m.Director.FirstName, m.Director.LastName)
                          : null
              ))
              .FirstOrDefaultAsync(ct);
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



    private static IQueryable<Movie> ApplySorting(IQueryable<Movie> query, string? sort)
    {
        return sort switch
        {
            "title_asc" => query.OrderBy(m => m.Title),
            "title_desc" => query.OrderByDescending(m => m.Title),
            "year_asc" => query.OrderBy(m => m.Year),
            "year_desc" => query.OrderByDescending(m => m.Year),
            _ => query.OrderBy(m => m.Id)
        };
    }

    private static IQueryable<Movie> ApplyFilters(
        IQueryable<Movie> query,
        string? search,
        string? genre,
        int? min_year,
        int? max_year)
    {

        if (min_year.HasValue)
            query = query.Where(m => m.Year >= min_year.Value);

        if (max_year.HasValue)
            query = query.Where(m => m.Year <= max_year.Value);

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(m => m.Genre == genre);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(m => EF.Functions.Like(m.Title, $"%{search.Trim()}%"));

        return query;
    }

}