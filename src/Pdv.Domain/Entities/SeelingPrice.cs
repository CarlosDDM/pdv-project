namespace Pdv.Domain.Entities;

public sealed class SeelingPrice
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SeelingPrice() { }
}
