using Pdv.Domain.Common;

namespace Pdv.Domain.Entities;

public sealed class SeelingPrice
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SeelingPrice() { }

    private SeelingPrice(Guid productId, decimal price) 
    {
        this.ProductId = productId;
        this.Price = price;
        this.CreatedAt = DateTime.UtcNow;
    }

    public static Result<SeelingPrice> Create(Guid productId, decimal price)
    {
        if (productId == Guid.Empty)
            return Result<SeelingPrice>.Fail("O Id do produto é obrigatório e não pode ser vazio.");

        if (price <= 0)
            return Result<SeelingPrice>.Fail("O preço não pode ser menor ou igual a 0.");

        return Result<SeelingPrice>.Ok(new SeelingPrice(productId, price));
    }
}
