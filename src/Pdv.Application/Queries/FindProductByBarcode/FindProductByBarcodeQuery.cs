using MediatR;
using Pdv.Application.DTOs.Responses;
using Pdv.Domain.Common;

namespace Pdv.Application.Queries.FindProductByBarcode;

public sealed record FindProductByBarcodeQuery(
    string Barcode
) : IRequest<Result<ProductResponse>>;
