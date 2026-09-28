using Microsoft.Extensions.Logging;
using NSubstitute;
using Pdv.Application.Interfaces;
using Pdv.Application.Queries.FindProduct;
using Pdv.Application.Tests.Factories;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Queries.FindProduct;

public sealed class FindProductHandlerTests
{
    private readonly IStorageService _storage = Substitute.For<IStorageService>();
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<FindProductHandler> _logger = Substitute.For<ILogger<FindProductHandler>>();
    private readonly FindProductHandler _handler;

    private static readonly Guid ProductId = Guid.Parse("9AC1A6F1-4A87-4624-B5F5-B74D494E21E0");

    public FindProductHandlerTests()
    {
        _handler = new FindProductHandler(_repository, _logger, _storage);
    }

    private static FindProductQuery CriarQueryValida() => new(Id: ProductId);

    [Fact]
    public async Task Handle_ComProdutoSemImagem_DeveRetornarProdutoSemChamarStorage()
    {
        var product = ProductFactory.CriarProductValido();
        var query = CriarQueryValida();

        _repository
            .FirstOrDefaultAsync(query.Id, Arg.Any<CancellationToken>())
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
            .FirstOrDefaultAsync(query.Id, Arg.Any<CancellationToken>());

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
            .FirstOrDefaultAsync(query.Id, Arg.Any<CancellationToken>())
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
            .FirstOrDefaultAsync(query.Id, Arg.Any<CancellationToken>());

        _storage
            .Received(1)
            .GetPublicUrl(product.ImageUrl!);
    }

    [Fact]
    public async Task Handle_ComProdutoNaoEncontrado_DeveFalharERetornarErro()
    {
        var query = CriarQueryValida();

        _repository
            .FirstOrDefaultAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Produto não encontrado.", result.Error);

        await _repository
            .Received(1)
            .FirstOrDefaultAsync(query.Id, Arg.Any<CancellationToken>());

        _storage
            .DidNotReceive()
            .GetPublicUrl(Arg.Any<string>());
    }
}