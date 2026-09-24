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
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    public DateTime? DeletedAt { get; private set; }


    private readonly List<SeelingPrice> _seelingPrices = [];
    private readonly List<OrderItem> _orderItems = [];
    private readonly List<SupplierPurchasing> _suppliersPurchasing = [];
    private readonly List<Stock> _stocks = [];

    public IReadOnlyCollection<SeelingPrice> SeelingPrices => _seelingPrices.AsReadOnly();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public IReadOnlyCollection<SupplierPurchasing> SuppliersPurchasing => _suppliersPurchasing.AsReadOnly();
    public IReadOnlyCollection<Stock> Stocks => _stocks.AsReadOnly();

    public SeelingPrice? CurrentPrice => _seelingPrices
        .OrderByDescending(sp => sp.CreatedAt)
        .ThenByDescending(sp => sp.Id)
        .FirstOrDefault();

    private Product() { }

    private Product(string name, ProductType type, string? brand = null, string? barcode = null)
    {
        this.Name = name;
        this.Type = type;
        this.Brand = brand;
        this.Barcode = barcode;
        this.CreatedAt = DateTime.UtcNow;
    }

    private Product UpdateProduct(string name, ProductType type, string? brand = null, string? barcode = null)
    {
        this.Name = name;
        this.Type = type;
        this.Brand = brand;
        this.Barcode = barcode;
        this.UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public static Result<Product> Create(string name, ProductType type, string? brand = null, string? barcode = null)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            return Result<Product>.Fail("O nome deve ter entre 1 e 100 caracteres");

        if (!Enum.IsDefined<ProductType>(type))
            return Result<Product>.Fail($"Tipo de produto inválido: {type}");

        if (brand is not null && (string.IsNullOrWhiteSpace(brand) || brand.Length > 50))
            return Result<Product>.Fail("A marca deve ter entre 1 e 50 caracteres");

        if (barcode is not null && (barcode.Length < 8 || barcode.Length > 20))
            return Result<Product>.Fail("O código de barras deve ter entre 8 e 20 caracteres");

        return Result<Product>.Ok(new Product(name, type, brand, barcode));
    }

    public Result Update(string name, ProductType type, string? brand = null, string? barcode = null)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
            return Result.Fail("O nome deve ter entre 1 e 100 caracteres");

        if (!Enum.IsDefined<ProductType>(type))
            return Result.Fail($"Tipo de produto inválido: {type}");

        if (brand is not null && (string.IsNullOrWhiteSpace(brand) || brand.Length > 50))
            return Result.Fail("A marca deve ter entre 1 e 50 caracteres");

        if (barcode is not null && (barcode.Length < 8 || barcode.Length > 20))
            return Result.Fail("O código de barras deve ter entre 8 e 20 caracteres");

        UpdateProduct(name, type, brand, barcode);
        return Result.Ok();
    }

    public void MarkAsDeleted()
    {
        if (this.IsDeleted) return;

       this.IsDeleted = true;
       this.DeletedAt = DateTime.UtcNow;
    }

    public Result SetImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return Result.Fail("Path da imagem inválida.");

        this.ImageUrl = imagePath;
        this.UpdatedAt = DateTime.UtcNow;
        return Result.Ok();
    }
}
