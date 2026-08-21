using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string? Cod { get; private set; }
    public string Name { get; private set; }
    public Guid StockId { get; private set; }
    public string? Brand { get; private set; }
    public ProductType Type { get; private set; }
    public string? ImageUrl{ get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<SeelingPrice> _seelingPrices = [];
    private readonly List<OrderItem> _orderItems = [];
    private readonly List<SupplierPurchasing> _suppliersPurchasing = [];

    public IReadOnlyCollection<SeelingPrice> SeelingPrices => _seelingPrices.AsReadOnly();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public IReadOnlyCollection<SupplierPurchasing> SuppliersPurchasing => _suppliersPurchasing.AsReadOnly();

    private Product() { }
}
