namespace Pdv.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Qtd { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<DevolutionItem> _devolutionItems = [];
    public IReadOnlyCollection<DevolutionItem> DevolutionItems => _devolutionItems.AsReadOnly();

    private OrderItem() { }
}
