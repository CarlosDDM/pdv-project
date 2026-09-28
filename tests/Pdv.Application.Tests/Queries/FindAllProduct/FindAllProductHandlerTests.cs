using Microsoft.Extensions.Logging;
using NSubstitute;
using Pdv.Application.Interfaces;
using Pdv.Application.Queries.FindAllProduct;
using Pdv.Application.Tests.Factories;
using Pdv.Domain.Common;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Queries.FindAllProduct;

public sealed class FindAllProductHandlerTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<FindAllProductHandler> _logger = Substitute.For<ILogger<FindAllProductHandler>>();
    private readonly IStorageService _storage = Substitute.For<IStorageService>();
    private readonly FindAllProductHandler _handler;

    public FindAllProductHandlerTests()
    {
        _handler = new FindAllProductHandler(_repository, _logger, _storage);
    }

    private static FindAllProductQuery CriarQueryValida(
        int page = 1,
        int pageSize = 10)
        => new() { Page = page, PageSize = pageSize };

    private static PagedResult<Product> CriarPagedResultValido(
        IReadOnlyList<Product> items,
        int page = 1,
        int pageSize = 10,
        int? totalCount = null)
        => new(items, page, pageSize, totalCount ?? items.Count);

    [Fact]
    public async Task Handle_ComListaVazia_DeveRetornarPaginacaoVaziaERepassarParametros()
    {
        var query = CriarQueryValida(page: 2, pageSize: 5);

        var pagedResult = CriarPagedResultValido([], page: query.Page, pageSize: query.PageSize, totalCount: 0);

        _repository
            .ListAllAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var response = await _handler.Handle(query, CancellationToken.None);

        Assert.Empty(response.Items);
        Assert.Equal(query.Page, response.Page);
        Assert.Equal(query.PageSize, response.PageSize);
        Assert.Equal(0, response.TotalCount);

        await _repository
            .Received(1)
            .ListAllAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComProdutoSemImagemENemPreco_DeveMapearCamposSemChamarStorage()
    {
        var query = CriarQueryValida();
        var product = ProductFactory.CriarProductValido();

        var pagedResult = CriarPagedResultValido([product], page: query.Page, pageSize: query.PageSize);

        _repository
            .ListAllAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var response = await _handler.Handle(query, CancellationToken.None);

        Assert.Single(response.Items);

        var item = response.Items[0];
        Assert.Equal(product.Id, item.Id);
        Assert.Equal(product.Name, item.Name);
        Assert.Equal(product.Type, item.Type);
        Assert.Equal(product.Brand, item.Brand);
        Assert.Equal(product.Barcode, item.Barcode);
        Assert.Null(item.ImageUrl);
        Assert.Null(item.CurrentPrice);

        _storage.DidNotReceive().GetPublicUrl(Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_ComProdutoComImagem_DeveRetornarUrlPublicaDaImagem()
    {
        var query = CriarQueryValida();
        var product = ProductFactory.CriarProductValido();
        product.SetImage("product/arroz.jpg");

        var pagedResult = CriarPagedResultValido([product], page: query.Page, pageSize: query.PageSize);

        _repository
            .ListAllAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        _storage
            .GetPublicUrl("product/arroz.jpg")
            .Returns("https://teste.com/product/arroz.jpg");

        var response = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal("https://teste.com/product/arroz.jpg", response.Items[0].ImageUrl);

        _storage.Received(1).GetPublicUrl("product/arroz.jpg");
    }

    [Fact]
    public async Task Handle_ComVariosProdutos_DeveMapearTodosNaOrdemRecebida()
    {
        var query = CriarQueryValida();
        var produto1 = ProductFactory.CriarProductValido(name: "Arroz 1Kg");
        var produto2 = ProductFactory.CriarProductValido(name: "Feijão 1Kg");

        var pagedResult = CriarPagedResultValido(
            [produto1, produto2],
            page: query.Page,
            pageSize: query.PageSize,
            totalCount: 2);

        _repository
            .ListAllAsync(query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(pagedResult);

        var response = await _handler.Handle(query, CancellationToken.None);

        Assert.Equal(2, response.Items.Count);
        Assert.Equal("Arroz 1Kg", response.Items[0].Name);
        Assert.Equal("Feijão 1Kg", response.Items[1].Name);
        Assert.Equal(2, response.TotalCount);
    }
}