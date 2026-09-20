namespace ChatApp.Common;

public record PagedResult<TItem, TCursor>(
    IReadOnlyList<TItem> Items,
    TCursor? NextCursor,
    bool HasMore
);
