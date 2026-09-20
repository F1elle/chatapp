using Microsoft.EntityFrameworkCore;

namespace ChatApp.Common.Extensions;

public static class QueryablePagingExtension
{
    public static async Task<PagedResult<TDto, TCursor>> ToPagedResult<TSource, TDto, TCursor>(
        this IQueryable<TSource> query,
        int pageSize,
        Func<TSource, TDto> projector,
        Func<TSource, TCursor> cursorSelector,
        CancellationToken ct = default
    )
    {
        var rawItems = await query.Take(pageSize + 1).ToListAsync(ct);
        bool hasMore = rawItems.Count > pageSize;
        var items = hasMore ? rawItems.Take(pageSize).ToList() : rawItems;

        return new PagedResult<TDto, TCursor>(
            items.Select(projector).ToList(),
            hasMore && items.Count > 0 ? cursorSelector(items[pageSize - 1]) : default,
            hasMore
        );
    }
}
