using MediatR;
using Microsoft.Extensions.Logging;
using Pdv.Application.DTOs.Responses;
using Pdv.Application.Interfaces;
using Pdv.Domain.Common;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Queries.FindProductByBarcode;

public sealed class FindProductByBarcodeHandler : IRequestHandler<FindProductByBarcodeQuery, Result<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<FindProductByBarcodeHandler> _logger;
    private readonly IStorageService _storage;

    public FindProductByBarcodeHandler(IProductRepository repository, ILogger<FindProductByBarcodeHandler> logger, IStorageService storage)
    {
        _repository = repository;
        _logger = logger;
        _storage = storage;
    }

    public async Task<Result<ProductResponse>> Handle(FindProductByBarcodeQuery query, CancellationToken ct)
    {
        _logger.LogInformation("Buscando produto. Barcode: {Barcode}", query.Barcode);
        var product = await _repository.FirstOrDefaultByBarcodeAsync(query.Barcode, ct);

        if (product is null)
        {
            _logger.LogWarning("Produto não encontrado. Barcode: {Barcode}", query.Barcode);
            return Result<ProductResponse>.Fail("Produto não encontrado.");
        }

        var current = product.CurrentPrice;

        var response = new ProductResponse(
            Id: product.Id,
            Barcode: product.Barcode,
            Name: product.Name,
            Brand: product.Brand,
            Type: product.Type,
            ImageUrl: product.ImageUrl is null 
                ? null
                : _storage.GetPublicUrl(product.ImageUrl),
            CurrentPrice: current is null 
                ? null 
                : new SeelingPriceResponse(
                    Id: current.Id,
                    Price: current.Price,
                    CreatedAt: current.CreatedAt
                ),
            CreatedAt: product.CreatedAt,
            UpdatedAt: product.UpdatedAt
        );

        return Result<ProductResponse>.Ok(response);
    }
}
