using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class Company
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string? Cnpj { get; private set; }
    public Guid? ReferedTo {  get; private set; }
    public CompanyType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<User> _users = [];
    private readonly List<Stock> _stocks = [];

    public IReadOnlyCollection<User> Users => _users.AsReadOnly();
    public IReadOnlyCollection<Stock> Stocks => _stocks.AsReadOnly();

    private Company() { }
}
