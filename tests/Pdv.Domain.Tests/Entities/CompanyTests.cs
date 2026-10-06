using Pdv.Domain.Entities;
using Pdv.Domain.Enums;

namespace Pdv.Domain.Tests.Entities;

public sealed class CompanyTests
{
    private const string name = "A casa do teste";
    private static Company CriarCompanyValido() 
    {
        var result = Company.Create(name, CompanyType.Matriz);

        var company = result.Value!;
        return company;
    }

    [Fact]
    public void Create_ComapnyComDadosValidos_DeveRetornarSucessoEPerssistir()
    {
        var start = DateTime.UtcNow;

        var result = Company.Create(name, CompanyType.Matriz);

        var finished = DateTime.UtcNow;

        Assert.True(result.IsSuccess);

        var company = result.Value!;

        Assert.Equal(name, company.Name);
        Assert.Equal(CompanyType.Matriz, company.Type);
        Assert.Null(company.Cnpj);
        Assert.Null(company.ReferedTo);

        Assert.InRange(company.CreatedAt ,start, finished);
    }

    [Fact]
    public void Create_CompanyComNomeMuitoGrande_DeveFalharERetornarError()
    {
        var result = Company.Create(new string('a', 101), CompanyType.Matriz);

        Assert.True(result.IsFailure);

        Assert.Equal("Nome não pode ser vazio, ou ele não pode maior que 100 caracteres.", result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_CompanyComNomeVazio_DeveFalharERetornarError(string name)
    {
        var result = Company.Create(name, CompanyType.Matriz);

        Assert.True(result.IsFailure);

        Assert.Equal("Nome não pode ser vazio, ou ele não pode maior que 100 caracteres.", result.Error);
    }

    [Fact]
    public void Create_CompanyComTypeNaoExistente_DeveFalharERetornarFalha()
    {
        var result = Company.Create(name, (CompanyType)999);

        Assert.True(result.IsFailure);

        Assert.Equal("Tipo de empresa inválido.", result.Error);
    }
}
