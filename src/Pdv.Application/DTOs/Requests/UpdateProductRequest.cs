using Pdv.Domain.Enums;

namespace Pdv.Application.DTOs.Requests;

public sealed record UpdateProductRequest(
    string Name,
    ProductType Type,
    string? Brand = null,
    string? Barcode = null
);
