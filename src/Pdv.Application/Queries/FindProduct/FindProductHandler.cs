using MediatR;
using Microsoft.Extensions.Logging;
using Pdv.Application.DTOs.Responses;
using Pdv.Application.Interfaces;
using Pdv.Domain.Common;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Queries.FindProduct;

public sealed class FindProductHandler : IRequestHandler<FindProductQuery, Result<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<FindProductHandler> _logger;
    private readonly IStorageService _storage;

    public FindProductHandler(IProductRepository repository, ILogger<FindProductHandler> logger, IStorageService storage)
    {
        _repository = repository;
        _logger = logger;
        _storage = storage;
    }

    public async Task<Result<ProductResponse>> Handle(FindProductQuery query, CancellationToken ct)
    {
        _logger.LogInformation("Buscando pedido. Id: {PedidoId}", query.Id);

        var product = await _repository.FirstOrDefaultAsync(query.Id, ct);

        if (product is null)
        {
            _logger.LogWarning("Produto não encontrado. Id: {ProdutoId}", query.Id);
            return Result<ProductResponse>.Fail("Produto não encontrado."); 
        }

        var current = product.CurrentPrice;

        var response = new ProductResponse(
            Id: product.Id,
            Barcode: product.Barcode,
            Name: product.Name,
            Brand: product.Brand,
            Type: product.Type,
            ImageUrl: product.ImageUrl is null ?
                null :
                _storage.GetPublicUrl(product.ImageUrl),
            CurrentPrice: current is null 
                ? null 
                : new SeelingPriceResponse(current.Id, current.Price, current.CreatedAt),
            CreatedAt: product.CreatedAt,
            UpdatedAt: product.UpdatedAt
        );

        return Result<ProductResponse>.Ok(response);  
    }
}
