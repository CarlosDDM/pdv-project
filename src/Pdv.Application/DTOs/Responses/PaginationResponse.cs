namespace Pdv.Application.DTOs.Responses;

public sealed record PaginationResponse<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
