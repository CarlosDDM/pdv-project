using Pdv.Domain.Common;

namespace Pdv.Domain.Entities;

public sealed class SeelingPrice
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SeelingPrice() { }

    private SeelingPrice(int productId, decimal price) 
    {
        this.ProductId = productId;
        this.Price = price;
        this.CreatedAt = DateTime.UtcNow;
    }

    public static Result<SeelingPrice> Create(int productId, decimal price)
    {
        if (productId <= 0)
            return Result<SeelingPrice>.Fail("O Id do produto é obrigatório e não pode ser negativo.");

        if (price <= 0)
            return Result<SeelingPrice>.Fail("O preço não pode ser menor ou igual a 0.");

        return Result<SeelingPrice>.Ok(new SeelingPrice(productId, price));
    }
}
