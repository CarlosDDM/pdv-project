using MediatR;
using Microsoft.Extensions.Logging;
using Pdv.Application.DTOs.Responses;
using Pdv.Application.Interfaces;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Queries.FindAllProduct;

public sealed class FindAllProductHandler : IRequestHandler<FindAllProductQuery, PaginationResponse<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<FindAllProductHandler> _logger;
    private readonly IStorageService _storage;
    public FindAllProductHandler(IProductRepository repository, ILogger<FindAllProductHandler> logger, IStorageService storage)
    {
        _repository = repository;
        _logger = logger;
        _storage = storage;
    }
    public async Task<PaginationResponse<ProductResponse>> Handle(FindAllProductQuery query, CancellationToken ct)
    {
        _logger.LogInformation("Buscando todos os produtos. Página: {PageNumber}, Tamanho da página: {PageSize}", query.Page, query.PageSize);
        var result = await _repository.ListAllAsync(query.Page, query.PageSize);

        _logger.LogInformation("Produtos listados. Total: {TotalCount}, Página: {PageNumber}, Tamanho da página: {PageSize}", result.TotalCount, result.Page, result.PageSize);
        var productResponses = result.Items.Select(p => new ProductResponse(
            p.Id,
            p.Barcode,
            p.Name,
            p.Brand,
            p.Type,
            p.ImageUrl is null 
                ? null
                : _storage.GetPublicUrl(p.ImageUrl),
            p.CurrentPrice is null 
                ? null 
                : new SeelingPriceResponse(
                    p.CurrentPrice.Id,
                    p.CurrentPrice.Price,
                    p.CurrentPrice.CreatedAt
                ),
            p.CreatedAt,
            p.UpdatedAt
            )).ToList();

        return new PaginationResponse<ProductResponse>(productResponses, result.Page, result.PageSize, result.TotalCount);
    }
}
