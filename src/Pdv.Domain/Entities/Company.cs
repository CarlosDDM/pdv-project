using Pdv.Domain.Common;
using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Cnpj { get; private set; }
    public Guid? ReferedTo { get; private set; }
    public CompanyType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<User> _users = [];
    private readonly List<Stock> _stocks = [];

    public IReadOnlyCollection<User> Users => _users.AsReadOnly();
    public IReadOnlyCollection<Stock> Stocks => _stocks.AsReadOnly();

    private Company() { }

    private Company (string name, CompanyType type, string? cnpj = null, Guid? referedTo = null)
    {
        this.Name = name;
        this.Cnpj = cnpj;
        this.Type = type;
        this.ReferedTo = referedTo;
        this.CreatedAt = DateTime.UtcNow;
    }

    private Company UpdateCompany(string name, CompanyType type, string? cnpj = null, Guid? referedTo = null)
    {
        this.Name = name;
        this.Cnpj = cnpj;
        this.Type = type;
        this.ReferedTo = referedTo;
        this.UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public static Result<Company> Create(string name, CompanyType type, string? cnpj = null, Guid? referedTo = null) 
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            return Result<Company>.Fail("Nome não pode ser vazio, ou ele não pode maior que 100 caracteres.");

        if (!Enum.IsDefined<CompanyType>(type))
            return Result<Company>.Fail("Tipo de empresa inválido.");

        return Result<Company>.Ok(new Company(name, type, cnpj, referedTo));
    }

    public Result Update(string name, CompanyType type, string? cnpj = null, Guid? referedTo = null) 
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            return Result<Company>.Fail("Nome não pode ser vazio, ou ele não pode maior que 100 caracteres.");

        if (!Enum.IsDefined<CompanyType>(type))
            return Result<Company>.Fail("Tipo de empresa inválido.");

        UpdateCompany(name, type, cnpj, referedTo);

        return Result.Ok();
    }
}
