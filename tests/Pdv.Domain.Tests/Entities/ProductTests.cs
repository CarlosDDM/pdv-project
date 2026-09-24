using Pdv.Domain.Entities;
using Pdv.Domain.Enums;

namespace Pdv.Domain.Tests.Entities;

public sealed class ProductTests
{
    private const string NomeValido = "Arroz soltinho 1Kg";

    private static Product CriarProductValido()
    {
        var result = Product.Create(NomeValido, ProductType.Unit);

        Assert.True(result.IsSuccess);
        return result.Value!;
    }

    [Fact]
    public void Create_ComDadosObrigatorios_DeveRetornarSucessoEPreencherCampos()
    {
        var antes = DateTime.UtcNow;

        var result = Product.Create(NomeValido, ProductType.Unit);

        var depois = DateTime.UtcNow;
        Assert.True(result.IsSuccess);

        var product = result.Value!;
        Assert.Equal(NomeValido, product.Name);
        Assert.Equal(ProductType.Unit, product.Type);
        Assert.Null(product.Barcode);
        Assert.Null(product.ImageUrl);
        Assert.Null(product.Brand);
        Assert.InRange(product.CreatedAt, antes, depois);
        Assert.False(product.IsDeleted);
        Assert.Null(product.DeletedAt);
    }

    [Fact]
    public void Create_ComTodosOsCampos_DevePopularMarcaECodigoDeBarras()
    {
        var result = Product.Create(NomeValido, ProductType.Unit, "Teste", "12345678");

        Assert.True(result.IsSuccess);

        var product = result.Value!;
        Assert.Equal("Teste", product.Brand);
        Assert.Equal("12345678", product.Barcode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_SemONome_DeveFalhar(string? name)
    {
        var result = Product.Create(name!, ProductType.Unit);

        Assert.True(result.IsFailure);
        Assert.Equal("O nome deve ter entre 1 e 100 caracteres", result.Error);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Create_ComNomeNoLimite_DeveSerValido(int length)
    {
        var result = Product.Create(new string('a', length), ProductType.Unit);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_ComNomeMuitoGrande_DeveFalhar()
    {
        var result = Product.Create(new string('a', 101), ProductType.Unit);

        Assert.True(result.IsFailure);
        Assert.Equal("O nome deve ter entre 1 e 100 caracteres", result.Error);
    }

    [Fact]
    public void Create_ComTipoNaoDefinido_DeveFalhar()
    {
        var result = Product.Create(NomeValido, (ProductType)999);

        Assert.True(result.IsFailure);
        Assert.Equal("Tipo de produto inválido: 999", result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ComMarcaEmBrancoOuVazio_DeveFalhar(string marca)
    {
        var result = Product.Create(NomeValido, ProductType.Unit, marca);

        Assert.True(result.IsFailure);
        Assert.Equal("A marca deve ter entre 1 e 50 caracteres", result.Error);
    }

    [Fact]
    public void Create_ComMarcaMuitoGrande_DeveFalhar()
    {
        var result = Product.Create(NomeValido, ProductType.Unit, new string('a', 51));

        Assert.True(result.IsFailure);
        Assert.Equal("A marca deve ter entre 1 e 50 caracteres", result.Error);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(20)]
    public void Create_ComCodigoDeBarrasNoLimite_DeveSerValido(int length)
    {
        var result = Product.Create(NomeValido, ProductType.Unit, barcode: new string('1', length));

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(7)]
    [InlineData(21)]
    public void Create_ComCodigoDeBarrasForaDoLimite_DeveriaFalhar(int length)
    {
        var result = Product.Create(NomeValido, ProductType.Unit, barcode: new string('1', length));

        Assert.True(result.IsFailure);
        Assert.Equal("O código de barras deve ter entre 8 e 20 caracteres", result.Error);
    }

    [Fact]
    public void Update_ComDadosValidos_DeveAlterarCamposEDefinirUpdatedAt()
    {
        var product = CriarProductValido();
        var antes = DateTime.UtcNow;

        var result = product.Update("Arroz 1Kg", ProductType.Unit, "Teste", "12345678");

        var depois = DateTime.UtcNow;
        Assert.True(result.IsSuccess);
        Assert.Equal("Arroz 1Kg", product.Name);
        Assert.Equal("Teste", product.Brand);
        Assert.Equal("12345678", product.Barcode);
        Assert.NotNull(product.UpdatedAt);
        Assert.InRange(product.UpdatedAt!.Value, antes, depois);
    }

    [Fact]
    public void Update_ComNomeInvalido_DeveFalharESemAlterarOProduto()
    {
        var produto = CriarProductValido();

        var result = produto.Update("", ProductType.Unit);

        Assert.True(result.IsFailure);
        Assert.Equal("O nome deve ter entre 1 e 100 caracteres", result.Error);
        Assert.Equal(NomeValido, produto.Name);
        Assert.Null(produto.UpdatedAt);
    }

    [Fact]
    public void Update_ComCodigoDeBarrasInvalido_DeveFalharESemAlterarOProduto()
    {
        var produto = CriarProductValido();

        var result = produto.Update("Feijão 1kg", ProductType.Unit, barcode: "123");

        Assert.True(result.IsFailure);
        Assert.Equal("O código de barras deve ter entre 8 e 20 caracteres", result.Error);
        Assert.Equal(NomeValido, produto.Name);
        Assert.Null(produto.Barcode);
    }

    [Fact]
    public void Update_ComMarcaInvalida_DeveFalharSemAlterarOProduto()
    {
        var product = CriarProductValido();

        var result = product.Update(NomeValido, ProductType.Unit, brand: " ");

        Assert.True(result.IsFailure);
        Assert.Equal("A marca deve ter entre 1 e 50 caracteres", result.Error);
        Assert.Null(product.Brand);
        Assert.Null(product.UpdatedAt);
    }

    [Fact]
    public void Update_ComTipoQueNaoExiste_DeveFalharSemEAtualizarUpdatedAt()
    {
        var product = CriarProductValido();

        var result = product.Update(NomeValido, (ProductType)999);

        Assert.True(result.IsFailure);
        Assert.Equal("Tipo de produto inválido: 999", result.Error);
        Assert.Equal(ProductType.Unit, product.Type);
        Assert.Null(product.UpdatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void SetImage_ComPathVazio_DeveFalharSemAlterarImagem(string? path)
    {
        var product = CriarProductValido();

        var result = product.SetImage(path!);

        Assert.True(result.IsFailure);
        Assert.Equal("Path da imagem inválida.", result.Error);
        Assert.Null(product.UpdatedAt);
        Assert.Null(product.ImageUrl);
    }

    [Fact]
    public void SetImage_ComPathValida_DeveGuardarOPathEDefinirUpdatedAt()
    {
        var product = CriarProductValido();

        var result = product.SetImage("product/arroz.jpg");

        Assert.True(result.IsSuccess);
        Assert.Equal("product/arroz.jpg", product.ImageUrl);
        Assert.NotNull(product.UpdatedAt);
    }

    [Fact]
    public void MarkAsDeleted_ProdutoAtivo_DeveMarcarComoExcluidoEDefinirDeletedAt()
    {
        var produto = CriarProductValido();

        produto.MarkAsDeleted();

        Assert.True(produto.IsDeleted);
        Assert.NotNull(produto.DeletedAt);
    }

    [Fact]
    public void MarkAsDeleted_ChamadoDuasVezes_NaoDeveAlterarDeletedAt()
    {
        var produto = CriarProductValido();
        produto.MarkAsDeleted();
        var primeiraData = produto.DeletedAt;

        produto.MarkAsDeleted();

        Assert.Equal(primeiraData, produto.DeletedAt);
    }
}