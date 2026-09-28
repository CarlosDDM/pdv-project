using Microsoft.Extensions.Logging;
using NSubstitute;
using Pdv.Application.Commands.UpdateProduct;
using Pdv.Application.Interfaces;
using Pdv.Application.Tests.Factories;
using Pdv.Domain.Entities;
using Pdv.Domain.Enums;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Commands.UpdateProduct;

public sealed class UpdateProductHandlerTests
{
    private const string NomeValido = "Arroz Soltinha 1Kg update";

    private static readonly Guid ProductId = Guid.Parse("9AC1A6F1-4A87-4624-B5F5-B74D494E21E0");

    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<UpdateProductHandler> _logger = Substitute.For<ILogger<UpdateProductHandler>>();
    private readonly IStorageService _storage = Substitute.For<IStorageService>();
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _handler = new UpdateProductHandler(_logger, _repository, _storage);
    }

    private static UpdateProductCommand CriarCommandValido(
        string name = NomeValido,
        ProductType type = ProductType.Unit,
        string? brand = null,
        string? barcode = null)
        => new(ProductId, name, type, brand, barcode);

    [Fact]
    public async Task Handle_ComBarcodeInalterado_DeveRetornarSucessoEAtualizarProdutoSemChecarDuplicidade()
    {
        var product = ProductFactory.CriarProductValido(barcode: "12345678");

        var command = CriarCommandValido(barcode: product.Barcode);

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _repository
            .UpdateAsync(product, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);

        var response = result.Value!;
        Assert.Equal(NomeValido, response.Name);
        Assert.Equal(command.Type, response.Type);
        Assert.Null(response.CurrentPrice);
        Assert.Null(response.ImageUrl);

        await _repository
            .Received(1)
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>());

        await _repository
            .Received(1)
            .UpdateAsync(product, Arg.Any<CancellationToken>());

        await _repository
            .DidNotReceive()
            .ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComBarcodeAlteradoParaUmInedito_DeveRetornarSucessoEChecarDuplicidade()
    {
        var product = ProductFactory.CriarProductValido(barcode: "12345678");

        var command = CriarCommandValido(barcode: "87654321");

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _repository
            .ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>())
            .Returns(false);

        _repository
            .UpdateAsync(product, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("87654321", result.Value!.Barcode);

        await _repository
            .Received(1)
            .ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>());

        await _repository
            .Received(1)
            .UpdateAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComBarcodeNulo_DeveRetornarSucessoSemChecarDuplicidade()
    {
        var product = ProductFactory.CriarProductValido(barcode: "12345678");

        var command = CriarCommandValido(barcode: null);

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _repository
            .UpdateAsync(product, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.Barcode);

        await _repository
            .DidNotReceive()
            .ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComProdutoComImagem_DeveRetornarUrlPublicaDaImagem()
    {
        var product = ProductFactory.CriarProductValido();
        var setImageResult = product.SetImage("images/arroz.jpg");
        Assert.True(setImageResult.IsSuccess);

        var command = CriarCommandValido(barcode: product.Barcode);

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _repository
            .UpdateAsync(product, Arg.Any<CancellationToken>())
            .Returns(product);

        _storage
            .GetPublicUrl("images/arroz.jpg")
            .Returns("https://teste.com/images/arroz.jpg");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("https://teste.com/images/arroz.jpg", result.Value!.ImageUrl);

        _storage
            .Received(1)
            .GetPublicUrl("images/arroz.jpg");
    }


    [Fact]
    public async Task Handle_ComProdutoNaoEncontrado_DeveFalhar()
    {
        var command = CriarCommandValido();

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Produto não encontrado.", result.Error);

        await _repository
            .DidNotReceive()
            .ExistsBarcodeAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _repository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComBarcodeAlteradoParaUmJaExistente_DeveFalhar()
    {
        var product = ProductFactory.CriarProductValido(barcode: "12345678");

        var command = CriarCommandValido(barcode: "87654321");

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _repository
            .ExistsBarcodeAsync(command.Barcode!, Arg.Any<CancellationToken>())
            .Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("O código de barras já existe.", result.Error);

        Assert.Equal("12345678", product.Barcode);

        await _repository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComNomeInvalido_DeveFalharSemPersistir()
    {
        var product = ProductFactory.CriarProductValido(barcode: "12345678");

        var command = CriarCommandValido(name: "", barcode: product.Barcode);

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("O nome deve ter entre 1 e 100 caracteres", result.Error);

        Assert.Equal(ProductFactory.NomeValido, product.Name);

        await _repository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComTipoNaoDefinido_DeveFalharSemPersistir()
    {
        var product = ProductFactory.CriarProductValido(barcode: "12345678");

        var command = CriarCommandValido(type: (ProductType)999, barcode: product.Barcode);

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Tipo de produto inválido: 999", result.Error);

        await _repository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }
}