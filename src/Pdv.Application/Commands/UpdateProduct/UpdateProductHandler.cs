using MediatR;
using Microsoft.Extensions.Logging;
using Pdv.Application.DTOs.Responses;
using Pdv.Application.Interfaces;
using Pdv.Domain.Common;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Commands.UpdateProduct;

public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
{
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly IProductRepository _repository;
    private readonly IStorageService _storage;


    public UpdateProductHandler(ILogger<UpdateProductHandler> logger, IProductRepository repository, IStorageService storage)
    {
        _logger = logger;
        _repository = repository;
        _storage = storage;
    }

    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Handling UpdateProductCommand para o produto: {ProductId}", command.Id);

        var product = await _repository.FirstOrDefaultAsync(command.Id, ct);
        if (product is null)
        {
            _logger.LogWarning("Produto com ID {ProductId} não encontrado.", command.Id);
            return Result<ProductResponse>.Fail("Produto não encontrado.");
        }

        if (command.Barcode is not null && command.Barcode != product.Barcode)
        {
            var existsBarcode = await _repository.ExistsBarcodeAsync(command.Barcode, ct);
            if (existsBarcode)
            {
                _logger.LogWarning("O código de barras {Barcode} já existe.", command.Barcode);
                return Result<ProductResponse>.Fail("O código de barras já existe.");
            }
        }

        var updateResult = product.Update(command.Name, command.Type, command.Brand, command.Barcode);
        if (updateResult.IsFailure)
        {
            _logger.LogWarning("Falha ao validar atualização do produto {ProductId}: {Error}", command.Id, updateResult.Error);
            return Result<ProductResponse>.Fail(updateResult.Error);
        }

        _logger.LogInformation("Salvando as alterações do produto com ID {ProductId}.", command.Id);
        var productUpdated = await _repository.UpdateAsync(product, ct);

        if (productUpdated is null)
        {
            _logger.LogError("Falha ao atualizar o produto com ID {ProductId}.", command.Id);
            return Result<ProductResponse>.Fail("Falha ao atualizar o produto.");
        }

        var current = productUpdated.CurrentPrice;

        var response = new ProductResponse(
            Id: productUpdated.Id,
            Barcode: productUpdated.Barcode,
            Name: productUpdated.Name,
            Brand: productUpdated.Brand,
            Type: productUpdated.Type,
            ImageUrl: productUpdated.ImageUrl is null 
                ? null
                : _storage.GetPublicUrl(productUpdated.ImageUrl),
            CurrentPrice: current is null
                ? null
                : new SeelingPriceResponse(
                    current.Id,
                    current.Price, 
                    current.CreatedAt
                    ),
            CreatedAt: productUpdated.CreatedAt,
            UpdatedAt: productUpdated.UpdatedAt
        );

      return Result<ProductResponse>.Ok(response);
    }
}
