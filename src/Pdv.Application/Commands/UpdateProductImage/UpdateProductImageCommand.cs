using MediatR;
using Pdv.Application.DTOs.Responses;
using Pdv.Domain.Common;

namespace Pdv.Application.Commands.UpdateProductImage;

public sealed record UpdateProductImageCommand
    (
        Guid Id,
        Stream File,
        string FileName,
        string ContentType
    ) : IRequest<Result<ProductUpdateImageResponse>>;