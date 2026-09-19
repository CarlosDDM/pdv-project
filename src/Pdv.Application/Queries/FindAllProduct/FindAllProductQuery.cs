using MediatR;
using Pdv.Application.DTOs.Requests;
using Pdv.Application.DTOs.Responses;

namespace Pdv.Application.Queries.FindAllProduct;

public sealed record FindAllProductQuery() : PagedRequest, IRequest<PaginationResponse<ProductResponse>>;
