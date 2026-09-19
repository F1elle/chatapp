using Microsoft.EntityFrameworkCore;

namespace ChatApp.Common.Extensions;

public static class QueryablePagingExtension
{
    public static async Task<TResult> ToPagedResult<TSource, TDto, TCursor, TResult>(
        this IQueryable<TSource> query,
        int pageSize,
        Func<TSource, TDto> projector,
        Func<TDto, TCursor> cursorSelector,
        CancellationToken ct = default
    )
        where TResult : PagedResult<TDto, TCursor>, new()
    {
        var rawItems = await query.Take(pageSize + 1).ToListAsync(ct);

        bool hasMore = rawItems.Count > pageSize;

        var itemsToProcess = hasMore ? rawItems.Take(pageSize) : rawItems;

        var dtos = itemsToProcess.Select(projector).ToList();

        TCursor? nextCursor = hasMore && dtos.Count > 0 ? cursorSelector(dtos[^1]) : default;

        return new TResult
        {
            Items = dtos,
            NextCursor = nextCursor,
            HasMore = hasMore,
        };
    }
}
