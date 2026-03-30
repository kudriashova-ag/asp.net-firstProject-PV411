using Microsoft.EntityFrameworkCore;
using MyApp.Helpers.QueryParameters;
using MyApp.Models;

namespace MyApp.Helpers.Queryable;

public static class MovieQueryableExtension
{
    public static IQueryable<Movie> ApplyFilters(
        this IQueryable<Movie> query,
        MovieQueryParameters movieParameters
    )
    {
        if (movieParameters.MinYear.HasValue)
            query = query.Where(m => m.Year >= movieParameters.MinYear.Value);

        if (movieParameters.MaxYear.HasValue)
            query = query.Where(m => m.Year <= movieParameters.MaxYear.Value);

        if (!string.IsNullOrWhiteSpace(movieParameters.Genre))
            query = query.Where(m => m.Genre == movieParameters.Genre);

        if (!string.IsNullOrWhiteSpace(movieParameters.Search))
            query = query.Where(m => EF.Functions.Like(m.Title, $"%{movieParameters.Search.Trim()}%"));

        return query;
    }


    public static IQueryable<Movie> ApplySorting(
        this IQueryable<Movie> query,
        string? sort
    )
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
}


