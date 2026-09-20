using Pdv.Domain.Entities;
using Pdv.Domain.Enums;

namespace Pdv.Domain.Tests.Entities;

public sealed class ProductTests
{
    private static Product CriarProductValido()
    {
        var result = Product.Create("Arroz soltinho 1Kg", ProductType.Unit);
        Assert.True(result.IsSuccess);
        return result.Value!;
    }

    [Fact]
    public void Create_ComDadosObrigatorios_DeveRetornarSucessoEPreencherCampos() 
    {
        var antes = DateTime.UtcNow;

        var result = Product.Create("Arroz soltinho 1Kg", ProductType.Unit);

        var depois = DateTime.UtcNow;
        Assert.True(result.IsSuccess);

        var product = result.Value!;
        Assert.Equal("Arroz soltinho 1Kg", product.Name);
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
        var result = Product.Create("Arroz soltinho 1Kg", ProductType.Unit, "Teste", "12345678");

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
        var result = Product.Create("Arroz soltinho 1Kg", (ProductType)999);

        Assert.True(result.IsFailure);
        Assert.Equal($"Tipo de produto inválido: {999}", result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ComMarcaEmBrancoOuVazio_DeveFalhar(string marca)
    {
        var result = Product.Create("Arroz soltinho 1Kg", ProductType.Unit, marca);
        Assert.True(result.IsFailure);
        Assert.Equal("A marca deve ter entre 1 e 50 caracteres", result.Error);
    }

    [Fact]
    public void Create_ComMarcaMuitoGrande_DeveFalhar()
    {
        var result = Product.Create("Arroz soltinho 1Kg", ProductType.Unit, new string('a', 51));
        Assert.True(result.IsFailure);
        Assert.Equal("A marca deve ter entre 1 e 50 caracteres", result.Error);
    }
}
