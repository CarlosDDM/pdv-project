namespace Pdv.Application.DTOs.Responses;

public sealed record SeelingPriceResponse(
    Guid Id,
    decimal Price,
    DateTime CreatedAt
);