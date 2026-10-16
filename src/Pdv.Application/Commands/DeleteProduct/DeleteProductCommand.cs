using MediatR;
using Pdv.Domain.Common;

namespace Pdv.Application.Commands.DeleteProduct;

public sealed record DeleteProductCommand(int Id) : IRequest<Result>;
