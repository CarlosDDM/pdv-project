using MediatR;
using Pdv.Application.DTOs.Responses;
using Pdv.Domain.Common;
using Pdv.Domain.Enums;

namespace Pdv.Application.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    ProductType Type,
    string? Brand,
    string? Barcode
) : IRequest<Result<ProductResponse>>;