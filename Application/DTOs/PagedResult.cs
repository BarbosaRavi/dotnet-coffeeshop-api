namespace CoffeeShopApi.Application.DTOs;

public record PagedRequest(int Page = 1, int PageSize = 20);

public record PagedResult<T> (IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => PageSize <=0  ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}