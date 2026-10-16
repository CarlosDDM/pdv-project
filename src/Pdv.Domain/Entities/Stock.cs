using Pdv.Domain.Common;

namespace Pdv.Domain.Entities;

public sealed class Stock
{
    public int Id { get; private set; }
    public int CompanyId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Stock() { }

    private Stock(int companyId, int productId, int quantity)
    {
        this.CompanyId = companyId;
        this.ProductId = productId;
        this.Quantity = quantity;
        this.CreatedAt = DateTime.UtcNow;
    }

    public static Result<Stock> Create(int companyId, int productId, int quantity)
    {
        if (companyId > 0)
            return Result<Stock>.Fail("O Id da company é obrigatório e não pode ser negativo.");

        if (productId > 0)
            return Result<Stock>.Fail("O id do product é obrigatório e não pode ser negativo.");

        if (quantity < 0)
            return Result<Stock>.Fail("Quantity não pode ser negativa.");

        return Result<Stock>.Ok(new Stock(companyId, productId, quantity));
    }
}
