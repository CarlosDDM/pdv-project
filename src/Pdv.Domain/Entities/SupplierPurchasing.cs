namespace Pdv.Domain.Entities;

public sealed class SupplierPurchasing
{
    public Guid Id { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SupplierPurchasing() { }
}
