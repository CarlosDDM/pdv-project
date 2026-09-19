using Pdv.Domain.Enums;

namespace Pdv.Application.DTOs.Requests;

public sealed record CreateProductRequest(
    string Name,
    ProductType Type,
    string? Brand = null,
    string? Barcode = null
);