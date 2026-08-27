using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string? Cod { get; private set; }
    public string Name { get; private set; } = string.Empty;
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

    private Product(string name, ProductType type, string? imageUrl = null, string? brand = null, string? cod = null)
    {
        this.Name = name;
        this.Type = type;
        this.ImageUrl = imageUrl;
        this.Brand = brand;
        this.Cod = cod;
        this.CreatedAt = DateTime.UtcNow;
    }

    public static void Criar() { }
}
