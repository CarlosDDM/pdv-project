using Pdv.Domain.Entities;

namespace Pdv.Domain.Tests.Entities;

public sealed class SeelingPriceTests
{
    private static readonly Guid ProductId = Guid.Parse("9AC1A6F1-4A87-4624-B5F5-B74D494E21E0");

    [Fact]
    public void Create_SeelingPriceComDadosValidos_DeveCriarSeelingPriceERetornar() 
    {
        var start = DateTime.UtcNow;
        var result = SeelingPrice.Create(ProductId, 20.5m);
        var finish = DateTime.UtcNow;

        Assert.True(result.IsSuccess);

        var seelingPrice = result.Value!;

        Assert.Equal(ProductId, seelingPrice.ProductId);
        Assert.Equal(20.5m, seelingPrice.Price);
        Assert.InRange(seelingPrice.CreatedAt, start, finish);
    }

    [Fact]
    public void Create_SeelingPriceProductIdInvalido_DeveRetornarError()
    {
        var result = SeelingPrice.Create(Guid.Empty, 20.5m);

        Assert.True(result.IsFailure);

        Assert.Equal("O Id do produto é obrigatório e não pode ser vazio.", result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-15.5)]
    public void Create_SeelingPriceComPriceZeradoOuNegativo_DeveRetornarError(decimal price)
    {
        var result = SeelingPrice.Create(ProductId, price);

        Assert.True(result.IsFailure);

        Assert.Equal("O preço não pode ser menor ou igual a 0.", result.Error);
    }
}
