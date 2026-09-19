namespace ChatApp.Common;

public abstract record PagedResult<TItem, TCursor>
{
    public IReadOnlyList<TItem> Items { get; set; } = [];
    public TCursor? NextCursor { get; set; }
    public bool HasMore { get; set; }
}
