using Pdv.Domain.Enums;

namespace Pdv.Application.DTOs.Responses;

public sealed record ProductResponse(
    int Id,
    string? Barcode,
    string Name,
    string? Brand,
    ProductType Type,
    string? ImageUrl,
    SeelingPriceResponse? CurrentPrice,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);