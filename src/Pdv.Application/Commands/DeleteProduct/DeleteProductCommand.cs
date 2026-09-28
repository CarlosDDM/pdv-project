using MediatR;
using Pdv.Domain.Common;

namespace Pdv.Application.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Result>;
