using Microsoft.Extensions.Logging;
using NSubstitute;
using Pdv.Application.Commands.CreateSeelingPrice;
using Pdv.Application.Tests.Factories;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Commands.CreateSeelingPrice;

public sealed class CreateSeelingPriceHandlerTests
{
    private readonly IProductRepository _productRepository = Substitute.For<IProductRepository>();
    private readonly ISeelingPriceRepository _seelingPriceRepository = Substitute.For<ISeelingPriceRepository>();
    private readonly ILogger<CreateSeelingPriceHandler> _logger = Substitute.For<ILogger<CreateSeelingPriceHandler>>();
    private readonly CreateSeelingPriceHandler _handler;

    private static readonly Guid ProductId = Guid.Parse("9AC1A6F1-4A87-4624-B5F5-B74D494E21E0");

    public CreateSeelingPriceHandlerTests()
    {
        _handler = new CreateSeelingPriceHandler(_seelingPriceRepository, _productRepository, _logger);
    }

    private static CreateSeelingPriceCommand CriarCommand(Guid? productId = null, decimal? price = null) 
    => new(productId ?? ProductId, price ?? 25.5m);

    [Fact]
    public async Task Handle_ComDadosValidos_DeveRetornarSucessoEPersistirSeelingPrice()
    {
        var product = ProductFactory.CriarProductValido();
        var command = CriarCommand();
        var seelingPrice = SeelingPriceFactory.Criar(command.ProductId, command.Price);

        var start = DateTime.UtcNow;

        _productRepository
            .FirstOrDefaultAsync(command.ProductId, Arg.Any<CancellationToken>())
            .Returns(product);

        _seelingPriceRepository
            .AddAsync(Arg.Any<SeelingPrice>(), Arg.Any<CancellationToken>())
            .Returns(x => x.Arg<SeelingPrice>());

        var result = await _handler.Handle(command, CancellationToken.None);

        var finish = DateTime.UtcNow;

        Assert.True(result.IsSuccess);

        var response = result.Value!;

        Assert.Equal(command.Price, response.Price);
        Assert.InRange(response.CreatedAt, start, finish);

        await _productRepository
            .Received(1)
            .FirstOrDefaultAsync(command.ProductId, Arg.Any<CancellationToken>());

        await _seelingPriceRepository
            .Received(1)
            .AddAsync(Arg.Is<SeelingPrice>(x => x.ProductId == command.ProductId && x.Price == command.Price), Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Handle_ComProductIdInvalido_DeveRetornarError()
    {
        var command = CriarCommand(Guid.Empty);

        _productRepository
            .FirstOrDefaultAsync(command.ProductId, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);

        Assert.Equal("O Product não existe ou não encontrado." , result.Error);

        await _productRepository
            .Received(1)
            .FirstOrDefaultAsync(command.ProductId, Arg.Any<CancellationToken>());

        await _seelingPriceRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<SeelingPrice>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-15.5)]
    public async Task Handle_ComPriceNegativoOuZero_DeveRetornarError(decimal price)
    {
        var product = ProductFactory.CriarProductValido();
        var command = CriarCommand(price: price);

        _productRepository
            .FirstOrDefaultAsync(command.ProductId, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);

        Assert.Equal("O preço não pode ser menor ou igual a 0.", result.Error);

        await _productRepository
            .Received(1)
            .FirstOrDefaultAsync(command.ProductId, Arg.Any<CancellationToken>());

        await _seelingPriceRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<SeelingPrice>(), Arg.Any<CancellationToken>());
    }
}
