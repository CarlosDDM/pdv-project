using Microsoft.Extensions.Logging;
using NSubstitute;
using Pdv.Application.Commands.DeleteProduct;
using Pdv.Application.Tests.Factories;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Commands.DeleteProduct;

public sealed class DeleteProductHandlerTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<DeleteProductHandler> _logger = Substitute.For<ILogger<DeleteProductHandler>>();
    private readonly DeleteProductHandler _handler;
    private static readonly Guid ProductId = Guid.Parse("9AC1A6F1-4A87-4624-B5F5-B74D494E21E0");

    public DeleteProductHandlerTests()
    {
        _handler = new DeleteProductHandler(_repository, _logger);
    }

    private static DeleteProductCommand CriarCommandValido() => new (Id: ProductId);

    [Fact]
    public async Task Handle_SoftDelete_DeveAlterarORegistroSemRetornarNada()
    {
        var command = CriarCommandValido();

        var product = ProductFactory.CriarProductValido();

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _repository
            .SoftDeleteAsync(product, Arg.Any<CancellationToken>())
            .Returns(true);

        var start = DateTime.UtcNow;

        var result = await _handler.Handle(command, CancellationToken.None);

        var finished = DateTime.UtcNow;

        Assert.True(result.IsSuccess);
        Assert.True(product.IsDeleted);
        Assert.InRange(product.DeletedAt!.Value, start, finished);

        await _repository
            .Received(1)
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>());

        await _repository
            .Received(1)
            .SoftDeleteAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SoftDeleteComIdNaoExistente_DeveFalhar()
    {
        var command = CriarCommandValido();

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Product não encontrado.", result.Error);

        await _repository
            .Received(1)
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>());

        await _repository
            .DidNotReceive()
            .SoftDeleteAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ComProdutoJaDeletado_DeveFalharSemChamarSoftDelete()
    {
        var command = CriarCommandValido();

        var product = ProductFactory.CriarProductValido();
        product.MarkAsDeleted();

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Product já foi apagado.", result.Error);

        await _repository
            .DidNotReceive()
            .SoftDeleteAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SoftDeleteTemProblemaAoSalvar_DeveFalhar()
    {
        var command = CriarCommandValido();

        var product = ProductFactory.CriarProductValido();

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _repository
            .SoftDeleteAsync(product, Arg.Any<CancellationToken>())
            .Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Erro ao deletar o produto.", result.Error);

        Assert.True(product.IsDeleted);

        await _repository
            .Received(1)
            .SoftDeleteAsync(product, Arg.Any<CancellationToken>());
    }
}