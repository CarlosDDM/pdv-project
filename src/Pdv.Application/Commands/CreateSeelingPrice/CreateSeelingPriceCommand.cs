using MediatR;
using Pdv.Application.DTOs.Responses;
using Pdv.Domain.Common;

namespace Pdv.Application.Commands.CreateSeelingPrice;

public sealed record CreateSeelingPriceCommand(
    int ProductId,
    decimal Price) : IRequest<Result<SeelingPriceResponse>>;
