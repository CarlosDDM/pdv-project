using Microsoft.Extensions.Logging;
using NSubstitute;
using Pdv.Application.Commands.UpdateProductImage;
using Pdv.Application.Interfaces;
using Pdv.Application.Tests.Factories;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Tests.Commands.UpdateProductImage;

public sealed class UpdateProductImageHandlerTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ILogger<UpdateProductImageHandler> _logger = Substitute.For<ILogger<UpdateProductImageHandler>>();
    private readonly IStorageService _storage = Substitute.For<IStorageService>();
    private readonly UpdateProductImageHandler _handler;

    private static readonly Guid ProductId = Guid.Parse("9AC1A6F1-4A87-4624-B5F5-B74D494E21E0");

    public UpdateProductImageHandlerTests()
    {
        _handler = new UpdateProductImageHandler(_storage, _repository, _logger);
    }

    private static UpdateProductImageCommand CriarCommandValido(
        string? fileName = null,
        string? contentType = null)
    {
        var defaultStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("conteudo_falso_de_imagem"));

        return new UpdateProductImageCommand(
            Id: ProductId ,
            File: defaultStream,
            FileName: fileName ?? "produto_imagem_teste.jpg",
            ContentType: contentType ?? "image/jpeg"
        );
    }

    [Fact]
    public async Task UpdateImage_DeveAtulizarOProdutoComOPathDaImagem_DeveRetornarSucessoEAtualizarProduto()
    {
        var product = ProductFactory.CriarProductValido();
        var command = CriarCommandValido();

        var start = DateTime.UtcNow;
        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _storage
            .UploadAsync(
                command.File,
                command.ContentType,
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
                )
            .Returns("product/teste.jpg");

        _repository
            .UpdateAsync(product, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        var finish = DateTime.UtcNow;

        Assert.True(result.IsSuccess);
        Assert.Equal("product/teste.jpg", product.ImageUrl);
        Assert.InRange(product.UpdatedAt!.Value, start, finish);

        await _repository
            .Received(1)
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>());

        await _repository
            .Received(1)
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _storage
            .Received(1)
            .UploadAsync(
                command.File,
                command.ContentType, 
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
                );

        await _storage.DidNotReceive()
            .DeleteAsync(
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
                );
    }

    [Fact]
    public async Task UpdateImage_DeveAtulizarOProdutoComOPathDaImagemAtual_DeveRetornarSucessoEAtualizarProduto()
    {
        var product = ProductFactory.CriarProductValido();

        product.SetImage("product/old.jpg");
        
        var command = CriarCommandValido();

        var start = DateTime.UtcNow;
        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _storage
            .UploadAsync(
                command.File,
                command.ContentType,
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
                )
            .Returns("product/teste.jpg");

        _repository
            .UpdateAsync(product, Arg.Any<CancellationToken>())
            .Returns(product);

        var result = await _handler.Handle(command, CancellationToken.None);

        var finish = DateTime.UtcNow;

        Assert.True(result.IsSuccess);
        Assert.Equal("product/teste.jpg", product.ImageUrl);
        Assert.InRange(product.UpdatedAt!.Value, start, finish);

        await _repository
            .Received(1)
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>());

        await _repository
            .Received(1)
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _storage
            .Received(1)
            .UploadAsync(
                command.File,
                command.ContentType,
                Arg.Any<string>(),
                Arg.Any<CancellationToken>()
                );

        await _storage.Received(1)
            .DeleteAsync(
                "product/old.jpg",
                Arg.Any<CancellationToken>()
                );
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

        await _storage
            .DidNotReceive()
            .UploadAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _repository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoUploadLancaExcecao_DeveFalharSemAlterarOuPersistirProduto()
    {
        var product = ProductFactory.CriarProductValido();
        var command = CriarCommandValido();

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _storage
            .UploadAsync(command.File, command.ContentType, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns<string>(_ => throw new InvalidOperationException("Falha simulada de upload"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Falha ao fazer upload da imagem.", result.Error);
        Assert.Null(product.ImageUrl); // não chegou a mudar

        await _repository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());

        await _storage
            .DidNotReceive()
            .DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoStorageDevolveKeyInvalida_DeveFalharComErroDoDominioSemPersistir()
    {
        var product = ProductFactory.CriarProductValido();
        var command = CriarCommandValido();

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _storage
            .UploadAsync(command.File, command.ContentType, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(" ");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("Path da imagem inválida.", result.Error);

        await _repository
            .DidNotReceive()
            .UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_QuandoDeleteDaImagemAntigaFalha_DeveRetornarSucessoMesmoAssim()
    {

        var product = ProductFactory.CriarProductValido();
        product.SetImage("product/old.jpg");

        var command = CriarCommandValido();

        _repository
            .FirstOrDefaultAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(product);

        _storage
            .UploadAsync(command.File, command.ContentType, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("product/teste.jpg");

        _storage
            .DeleteAsync("product/old.jpg", Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new InvalidOperationException("Falha simulada ao apagar")));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("product/teste.jpg", product.ImageUrl);

        await _repository
            .Received(1)
            .UpdateAsync(product, Arg.Any<CancellationToken>());
    }
}
