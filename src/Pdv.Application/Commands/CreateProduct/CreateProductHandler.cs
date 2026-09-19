using MediatR;
using Microsoft.Extensions.Logging;
using Pdv.Application.DTOs.Responses;
using Pdv.Domain.Common;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Commands.CreateProduct;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(IProductRepository repository, ILogger<CreateProductHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<ProductResponse>> Handle(CreateProductCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Handling CreateProductCommand para o product: {ProductName}", command.Name);

        if (command.Barcode is not null)
        {
            var existsBarcode =  await _repository.ExistsBarcodeAsync(command.Barcode, ct);
            if (existsBarcode)
            {
                _logger.LogWarning("O código de barras {Barcode} já existe.", command.Barcode);
                return Result<ProductResponse>.Fail("O código de barras já existe.");
            }
        }

        var productResult = Product.Create(command.Name, command.Type, command.Brand, command.Barcode);


        if (productResult.IsFailure)
        {
            _logger.LogWarning("Falha ao criar o produto: {Error}", productResult.Error);
            return Result<ProductResponse>.Fail(productResult.Error);
        }

        var product = productResult.Value!;

        await _repository.AddAsync(product, ct);

        _logger.LogInformation("Product criado com sucesso. Id: {ProductId}", product.Id);

        var current = product.CurrentPrice;

        var response = new ProductResponse(
            Id: product.Id,
            Barcode: product.Barcode,
            Name: product.Name,
            Brand: product.Brand,
            Type: product.Type,
            ImageUrl: product.ImageUrl,
            CurrentPrice: current is null
                ? null
                : new SeelingPriceResponse(current.Id, current.Price, current.CreatedAt),
            CreatedAt: product.CreatedAt,
            UpdatedAt: product.UpdatedAt
            );

        return Result<ProductResponse>.Ok(response);

    }
}
