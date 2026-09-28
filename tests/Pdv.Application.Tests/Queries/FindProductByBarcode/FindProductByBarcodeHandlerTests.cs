using Microsoft.Extensions.Logging;
using NSubstitute;
using Pdv.Application.Interfaces;
using Pdv.Application.Queries.FindProductByBarcode;
using Pdv.Application.Tests.Factories;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Queries.FindProductByBarcode;

public sealed class FindProductByBarcodeHandlerTests
{
    private readonly IStorageService _storage = Substitute.For<IStorageService>();
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<FindProductByBarcodeHandler> _logger = Substitute.For<ILogger<FindProductByBarcodeHandler>>();
    private readonly FindProductByBarcodeHandler _handler;

    private const string BarcodeValido = "7891234567890";

    public FindProductByBarcodeHandlerTests()
    {
        _handler = new FindProductByBarcodeHandler(_repository, _logger, _storage);
    }

    private static FindProductByBarcodeQuery CriarQueryValida() => new(Barcode: BarcodeValido);

    [Fact]
    public async Task Handle_ComProdutoSemImagem_DeveRetornarProdutoSemChamarStorage()
    {
        var product = ProductFactory.CriarProductValido();
        var query = CriarQueryValida();

        _repository
            .FirstOrDefaultByBarcodeAsync(query.Barcode, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);

        var response = result.Value!;
        Assert.Equal(product.Name, response.Name);
        Assert.Equal(product.Type, response.Type);
        Assert.Null(response.Barcode);
        Assert.Null(response.Brand);
        Assert.Null(response.CurrentPrice);
        Assert.Null(response.ImageUrl);

        await _repository
            .Received(1)
            .FirstOrDefaultByBarcodeAsync(query.Barcode, Arg.Any<CancellationToken>());

        _storage
            .DidNotReceive()
            .GetPublicUrl(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_ComProdutoComImagem_DeveRetornarUrlPublicaDaImagem()
    {
        var product = ProductFactory.CriarProductValido();
        product.SetImage("product/arroz.jpg");

        var query = CriarQueryValida();

        _repository
            .FirstOrDefaultByBarcodeAsync(query.Barcode, Arg.Any<CancellationToken>())
            .Returns(product);

        _storage
            .GetPublicUrl(product.ImageUrl!)
            .Returns("https://teste.com/product/arroz.jpg");

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);

        var response = result.Value!;
        Assert.Equal(product.Name, response.Name);
        Assert.Equal(product.Type, response.Type);
        Assert.Equal("https://teste.com/product/arroz.jpg", response.ImageUrl);
        Assert.Null(response.Barcode);
        Assert.Null(response.Brand);
        Assert.Null(response.CurrentPrice);

        await _repository
            .Received(1)
            .FirstOrDefaultByBarcodeAsync(query.Barcode, Arg.Any<CancellationToken>());

        _storage
            .Received(1)
            .GetPublicUrl(product.ImageUrl!);
    }

    [Fact]
    public async Task Handle_ComProdutoNaoEncontrado_DeveFalharERetornarErro()
    {
        var query = CriarQueryValida();

        _repository
            .FirstOrDefaultByBarcodeAsync(query.Barcode, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Produto não encontrado.", result.Error);

        await _repository
            .Received(1)
            .FirstOrDefaultByBarcodeAsync(query.Barcode, Arg.Any<CancellationToken>());

        _storage
            .DidNotReceive()
            .GetPublicUrl(Arg.Any<string>());
    }
}