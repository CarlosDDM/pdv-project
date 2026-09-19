namespace Pdv.Application.DTOs.Requests;

public abstract record PagedRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 30;
};