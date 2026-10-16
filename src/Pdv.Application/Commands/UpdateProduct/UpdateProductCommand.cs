using MediatR;
using Pdv.Application.DTOs.Responses;
using Pdv.Domain.Common;
using Pdv.Domain.Enums;

namespace Pdv.Application.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    int Id,
    string Name,
    ProductType Type,
    string? Brand,
    string? Barcode
) : IRequest<Result<ProductResponse>>;

