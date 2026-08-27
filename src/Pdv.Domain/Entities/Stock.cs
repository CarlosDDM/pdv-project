namespace Pdv.Domain.Entities;  

public sealed class Stock
{
    public Guid Id { get; private set; }
    public Guid CompanyId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }


    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Stock() { }
}
