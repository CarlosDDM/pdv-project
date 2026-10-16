using MediatR;
using Pdv.Application.DTOs.Responses;
using Pdv.Domain.Common;

namespace Pdv.Application.Queries.FindProduct;

public sealed record FindProductQuery(int Id) : IRequest<Result<ProductResponse>>;
