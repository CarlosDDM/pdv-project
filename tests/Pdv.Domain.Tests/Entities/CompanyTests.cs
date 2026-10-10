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

        Assert.InRange(company.CreatedAt, start, finished);
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

    [Fact]
    public void Create_NomeComExatamente100Caracteres_DeveRetornarSucesso()
    {
        var result = Company.Create(new string('a', 100), CompanyType.Matriz);

        Assert.True(result.IsSuccess);
        Assert.Equal(100, result.Value!.Name.Length);
    }

    [Fact]
    public void Create_CnpjComMaisDe14Caracteres_DeveFalharERetornarError()
    {
        var result = Company.Create(name, CompanyType.Matriz, new string('1', 15));

        Assert.True(result.IsFailure);
        Assert.Equal("CNPJ inválido.", result.Error);
    }

    [Fact]
    public void Create_CnpjComExatamente14Caracteres_DeveRetornarSucessoEPersistir()
    {
        var cnpj = new string('1', 14);

        var result = Company.Create(name, CompanyType.Matriz, cnpj);

        Assert.True(result.IsSuccess);
        Assert.Equal(cnpj, result.Value!.Cnpj);
    }

    [Fact]
    public void Create_ReferedToValido_DeveRetornarSucessoEPersistir()
    {
        var referedTo = Guid.NewGuid();

        var result = Company.Create(name, CompanyType.Matriz, referedTo: referedTo);

        Assert.True(result.IsSuccess);
        Assert.Equal(referedTo, result.Value!.ReferedTo);
    }

    [Fact]
    public void Create_ReferedToGuidVazio_DeveFalharERetornarError()
    {
        var result = Company.Create(name, CompanyType.Matriz, referedTo: Guid.Empty);

        Assert.True(result.IsFailure);
        Assert.Equal("Id inválido.", result.Error);
    }

    [Fact]
    public void Create_CompanyValida_UpdatedAtDeveIniciarNulo()
    {
        var company = CriarCompanyValido();

        Assert.Null(company.UpdatedAt);
    }

    [Fact]
    public void Create_CompanyValida_UsersEStocksDevemIniciarVazios()
    {
        var company = CriarCompanyValido();

        Assert.Empty(company.Users);
        Assert.Empty(company.Stocks);
    }


    [Fact]
    public void Update_DadosValidos_DeveAtualizarCamposEDefinirUpdatedAt()
    {
        var company = CriarCompanyValido();
        var createdAt = company.CreatedAt;
        var referedTo = Guid.NewGuid();
        var cnpj = new string('2', 14);
        var start = DateTime.UtcNow;

        var result = company.Update("Novo nome", CompanyType.Matriz, cnpj, referedTo);

        var finished = DateTime.UtcNow;

        Assert.True(result.IsSuccess);
        Assert.Equal("Novo nome", company.Name);
        Assert.Equal(CompanyType.Matriz, company.Type);
        Assert.Equal(cnpj, company.Cnpj);
        Assert.Equal(referedTo, company.ReferedTo);
        Assert.NotNull(company.UpdatedAt);
        Assert.InRange(company.UpdatedAt!.Value, start, finished);
        Assert.Equal(createdAt, company.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Update_NomeVazio_DeveFalharERetornarError(string novoNome)
    {
        var company = CriarCompanyValido();

        var result = company.Update(novoNome, CompanyType.Matriz);

        Assert.True(result.IsFailure);
        Assert.Equal("Nome não pode ser vazio, ou ele não pode maior que 100 caracteres.", result.Error);
    }

    [Fact]
    public void Update_NomeMuitoGrande_DeveFalharERetornarError()
    {
        var company = CriarCompanyValido();

        var result = company.Update(new string('a', 101), CompanyType.Matriz);

        Assert.True(result.IsFailure);
        Assert.Equal("Nome não pode ser vazio, ou ele não pode maior que 100 caracteres.", result.Error);
    }

    [Fact]
    public void Update_NomeComExatamente100Caracteres_DeveRetornarSucesso()
    {
        var company = CriarCompanyValido();

        var result = company.Update(new string('a', 100), CompanyType.Matriz);

        Assert.True(result.IsSuccess);
        Assert.Equal(100, company.Name.Length);
    }

    [Fact]
    public void Update_TypeNaoExistente_DeveFalharERetornarError()
    {
        var company = CriarCompanyValido();

        var result = company.Update(name, (CompanyType)999);

        Assert.True(result.IsFailure);
        Assert.Equal("Tipo de empresa inválido.", result.Error);
    }

    [Fact]
    public void Update_CnpjComMaisDe14Caracteres_DeveFalharERetornarError()
    {
        var company = CriarCompanyValido();

        var result = company.Update(name, CompanyType.Matriz, new string('1', 15));

        Assert.True(result.IsFailure);
        Assert.Equal("CNPJ inválido.", result.Error);
    }

    [Fact]
    public void Update_ReferedToGuidVazio_DeveFalharERetornarError()
    {
        var company = CriarCompanyValido();

        var result = company.Update(name, CompanyType.Matriz, referedTo: Guid.Empty);

        Assert.True(result.IsFailure);
        Assert.Equal("Id inválido.", result.Error);
    }

    [Fact]
    public void Update_SemCnpjEReferedTo_DeveLimparValoresAnteriores()
    {
        var company = Company.Create(name, CompanyType.Matriz, new string('1', 14), Guid.NewGuid()).Value!;

        var result = company.Update(name, CompanyType.Matriz);

        Assert.True(result.IsSuccess);
        Assert.Null(company.Cnpj);
        Assert.Null(company.ReferedTo);
    }

    [Fact]
    public void Update_DadosInvalidos_NaoDeveAlterarEstadoDaEntidade()
    {
        var company = CriarCompanyValido();

        var result = company.Update(new string('a', 101), CompanyType.Matriz, new string('1', 14), Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(name, company.Name);
        Assert.Null(company.Cnpj);
        Assert.Null(company.ReferedTo);
        Assert.Null(company.UpdatedAt);
    }

    [Fact]
    public void DisableCompany_ComIsDisabledFalse_DeveDesabilitarCompany()
    {
        var company = CriarCompanyValido();

        var result = company.DisableCompany();

        Assert.True(result.IsSuccess);
        Assert.True(company.IsDisabled);
        Assert.NotNull(company.UpdatedAt);
    }

    [Fact]
    public void DisableCompany_ComIsDisabledTrue_DeveRetornarError()
    {
        var company = CriarCompanyValido();
        company.DisableCompany();

        var result = company.DisableCompany();

        Assert.True(result.IsFailure);
        Assert.Equal("Company já desabilitada.", result.Error);
    }
}