using Microsoft.Extensions.Logging;
using NSubstitute;
using Pdv.Application.Commands.CreateProduct;
using Pdv.Domain.Entities;
using Pdv.Domain.Enums;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Commands.CreateProduct;

public sealed class CreateProductHandlerTests
{
    private const string NomeValido = "Arroz soltinha 1Kg";

    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<CreateProductHandler> _logger = Substitute.For<ILogger<CreateProductHandler>>();
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _handler = new CreateProductHandler(_repository, _logger);
    }

    private static CreateProductCommand CriarCommandValido(
        string name = NomeValido,
        ProductType type = ProductType.Unit,
        string? brand = null,
        string? barcode = null)
        => new(name, type, brand, barcode);

    [Fact]
    public async Task Handle_ComDadosValidosSemBarcodeEBrand_DeveRetornarSucessoEPersistirProduto()
    {
        var command = CriarCommandValido();

        var start = DateTime.UtcNow;

        var result = await _handler.Handle(command, CancellationToken.None);

        var finish = DateTime.UtcNow;

        Assert.True(result.IsSuccess);

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
        var command = CriarCommandValido(barcode: "12345678");

        _repository
            .ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>())
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
    public async Task Handle_ComBarcodeJaExistente_DeveRetornarErroERetornarOMotivo()
    {
        var command = CriarCommandValido(barcode: "12345678");

        _repository
            .ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("O código de barras já existe.", result.Error);

        await _repository.Received(1).ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComNameInvalido_DeveRetornarErroERetornarOMotivo()
    {
        var command = CriarCommandValido(name: "");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("O nome deve ter entre 1 e 100 caracteres", result.Error);

        await _repository.DidNotReceive().ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComTypeNaoValido_DeveRetornarErroERetornarOMotivo()
    {
        var command = CriarCommandValido(type: (ProductType)999);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Tipo de produto inválido: 999", result.Error);

        await _repository.DidNotReceive().ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(7)]
    [InlineData(21)]
    public async Task Handle_ComBarcodeForaDoLimite_DeveRetornarErroERetornarOMotivo(int length)
    {
        var command = CriarCommandValido(barcode: new string('2', length));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("O código de barras deve ter entre 8 e 20 caracteres", result.Error);

        await _repository.Received(1).ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComBrandVazia_DeveRetornarErroERetornarOMotivo()
    {
        var command = CriarCommandValido(brand: "");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("A marca deve ter entre 1 e 50 caracteres", result.Error);

        await _repository.DidNotReceive().ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComBrandMaiorQueOLimite_DeveRetornarErroERetornarOMotivo()
    {
        var command = CriarCommandValido(brand: new string('A', 51));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("A marca deve ter entre 1 e 50 caracteres", result.Error);

        await _repository.DidNotReceive().ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComBrandValida_DeveRetornarSucessoEPersistirProduto()
    {
        var command = CriarCommandValido(brand: "Soltinho");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);

        var response = result.Value!;
        Assert.Equal(command.Brand, response.Brand);

        await _repository.DidNotReceive().ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _repository.Received(1).AddAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}