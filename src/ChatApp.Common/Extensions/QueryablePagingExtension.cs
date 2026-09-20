using Microsoft.EntityFrameworkCore;

namespace ChatApp.Common.Extensions;

public static class QueryablePagingExtension
{
    public static async Task<TResult> ToPagedResult<TSource, TDto, TCursor, TResult>(
        this IQueryable<TSource> query,
        int pageSize,
        Func<TSource, TDto> projector,
        Func<TSource, TCursor> cursorSelector,
        TResult? _ = default,
        CancellationToken ct = default
    )
        where TResult : PagedResult<TDto, TCursor>, new()
    {
        var rawItems = await query.Take(pageSize + 1).ToListAsync(ct);

        bool hasMore = rawItems.Count > pageSize;

        var items = hasMore ? rawItems.Take(pageSize).ToList() : rawItems;

        return new TResult
        {
            Items = items.Select(projector).ToList(),
            NextCursor = hasMore && items.Count > 0 ? cursorSelector(items[^1]) : default,
            HasMore = hasMore,
        };
    }
}
