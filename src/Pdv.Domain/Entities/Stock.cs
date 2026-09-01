using Pdv.Domain.Common;

namespace Pdv.Domain.Entities;  

public sealed class Stock
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Stock() { }

    private Stock(Guid companyId, Guid productId, int quantity)
    {
        this.CompanyId = companyId;
        this.ProductId = productId;
        this.Quantity = quantity;
        this.CreatedAt = DateTime.UtcNow;
    }

    public static Result<Stock> Create(Guid companyId, Guid productId, int quantity)
    {
        if (companyId == Guid.Empty)
            return Result<Stock>.Fail("O id da company é obrigatório.");

        if (productId == Guid.Empty)
            return Result<Stock>.Fail("O id do product é obrigatório.");

        if (quantity < 0)
            return Result<Stock>.Fail("Quantity não pode ser negativa.");

        return Result<Stock>.Ok(new Stock(companyId, productId, quantity));
    }
}
