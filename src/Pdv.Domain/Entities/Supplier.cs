namespace Pdv.Domain.Entities;

public sealed class Supplier
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Cnpj { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<SupplierPurchasing> _suppliersPurchasing = [];
    public IReadOnlyCollection<SupplierPurchasing> SupplierPurchasings => _suppliersPurchasing.AsReadOnly();

    private Supplier() { }
}
