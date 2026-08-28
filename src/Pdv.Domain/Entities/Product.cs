using Pdv.Domain.Common;
using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; }
    public string? Barcode { get; private set; }
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

    private Product(string name, ProductType type, string? brand = null, string? barcode = null)
    {
        this.Name = name;
        this.Type = type;
        this.Brand = brand;
        this.Barcode = barcode;
        this.CreatedAt = DateTime.UtcNow;
    }

    public static Result<Product> Create(string name, ProductType type, string? brand = null, string? barcode = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Fail("O nome não pode ser vazio");

        if (!Enum.IsDefined<ProductType>(type))
            return Result<Product>.Fail($"Tipo de produto inválido: {type}");

        if (brand is not null && string.IsNullOrWhiteSpace(brand))
            return Result<Product>.Fail("A marca não pode ser vazia");

        if (barcode is not null && (barcode.Length < 8 || barcode.Length > 20))
            return Result<Product>.Fail("O código de barras deve ter entre 8 e 20 caracteres");

        return Result<Product>.Ok(new Product(name, type, brand, barcode));
    }

    public Result SetImage(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return Result.Fail("URL da imagem inválida.");

        this.ImageUrl = imageUrl;
        this.UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }
}
