namespace MyApp.Helpers.Pagination;

using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoMapper;


public static class PaginationHelper
{
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>
    (
        this IQueryable<TEntity> query,
        int page,
        int size,
        IConfigurationProvider mapperConfig,
        CancellationToken ct
    )
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        int totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ProjectTo<TDto>(mapperConfig)
            .ToListAsync(ct);

        return new PagedResult<TDto>(items, totalCount, page, size);
    }
}
