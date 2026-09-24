using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Pdv.Application.Commands.CreateProduct;
using Pdv.Domain.Common;
using Pdv.Domain.Entities;
using Pdv.Domain.Enums;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Commands.CreateProduct;

public sealed class CreateProductHandlerTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<CreateProductHandler> _logger = Substitute.For<ILogger<CreateProductHandler>>();
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _handler = new CreateProductHandler(_repository, _logger);
    }


    [Fact]
    public async Task Handle_ComDadosValidosSemBarcodeEBrand_DeveRetornarSucessoEPersistirProduto()
    {
        var command = new CreateProductCommand(
            Name: "Arroz soltinha 1Kg",
            Type: ProductType.Unit,
            Brand: null,
            Barcode: null
            );

        var start = DateTime.UtcNow;

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);

        var finish = DateTime.UtcNow;

        var response = result.Value!;

        Assert.Equal(command.Name, response.Name);
        Assert.Equal(command.Type, response.Type);
        Assert.Null(response.Brand);
        Assert.Null(response.Barcode);
        Assert.InRange(response.CreatedAt, start, finish);

        Assert.Null(response.CurrentPrice);

        await _repository.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_ComDadosValidosComBarcode_DeveRetornarSucessoEPersistirProduto()
    {
        var command = new CreateProductCommand(
            Name: "Arroz soltinha 1Kg",
            Type: ProductType.Unit,
            Brand: null,
            Barcode: "12345678"
            );

        _repository
            .ExistsBarcodeAsync(
                command.Barcode!,
                Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);

        var response = result.Value!;

        Assert.Equal(command.Barcode, response.Barcode);
        Assert.Null(response.Brand);

        await _repository.Received(1).ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>());
        await _repository.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComDadosValidosComBarcode_DeveRetornarErrorERetornarOMotivo()
    {
        var command = new CreateProductCommand(
            Name: "Arroz soltinha 1Kg",
            Type: ProductType.Unit,
            Brand: null,
            Barcode: "12345678"
            );

        _repository
            .ExistsBarcodeAsync(
                command.Barcode!,
                Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);

        Assert.Equal("O código de barras já existe.", result.Error);

        await _repository.Received(1).ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>()) ;
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}
