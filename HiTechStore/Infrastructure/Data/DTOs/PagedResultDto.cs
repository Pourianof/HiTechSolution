namespace HiTechStore.Infrastructure.Data.DTOs;

public class PagedResultDto<T>
{

    public static PagedResultDto<T> Empty()
    {
        return new()
        {
            Items = [],
            PageNumber = 1,
            PageSize = 0,
            TotalCount = 0
        };
    }

    public int PageSize { get; set; } // it equivalent to limit for page
    public int PageNumber { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize == 0 ? 1 : (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
    public IEnumerable<T> Items { get; set; } = new List<T>();
}
