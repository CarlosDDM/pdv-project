namespace Pdv.Application.DTOs.Responses;

public sealed record SeelingPriceResponse(
    int Id,
    decimal Price,
    DateTime CreatedAt
);