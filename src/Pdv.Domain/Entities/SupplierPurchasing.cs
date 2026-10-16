namespace Pdv.Domain.Entities;

public sealed class SupplierPurchasing
{
    public int Id { get; private set; }
    public int SupplierId { get; private set; }
    public int ProductId { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SupplierPurchasing() { }
}
