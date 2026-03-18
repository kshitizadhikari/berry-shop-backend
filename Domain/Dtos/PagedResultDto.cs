namespace Domain.Dtos;

public class PagedResultDto<T>
{
    public List<T> Items { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;

    public static PagedResultDto<T> Create(IEnumerable<T> items, int totalCount, int page, int pageSize) => new()
    {
        Items = items.ToList(),
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    };
}