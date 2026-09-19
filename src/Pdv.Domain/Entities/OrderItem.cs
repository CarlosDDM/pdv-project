using Pdv.Domain.Common;

namespace Pdv.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Discount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<DevolutionItem> _devolutionItems = [];
    public IReadOnlyCollection<DevolutionItem> DevolutionItems => _devolutionItems.AsReadOnly();

    private OrderItem(Guid orderId, Guid productId, int quantity, decimal unitPrice, decimal discount)
    {
        this.OrderId = orderId;
        this.ProductId = productId;
        this.Quantity = quantity;
        this.UnitPrice = unitPrice;
        this.Discount = discount;
        this.CreatedAt = DateTime.UtcNow;
    }

    public static Result<OrderItem> Create(Guid orderId, Guid productId, int quantity, decimal unitPrice, decimal discount)
    {
        if (orderId == Guid.Empty)
            return Result<OrderItem>.Fail("OrderId não pode ser vazio.");

        if (productId == Guid.Empty)
            return Result<OrderItem>.Fail("ProductId não pode ser vazio.");

        if (quantity <= 0)
            return Result<OrderItem>.Fail("Quantity precisa ser um valor positivo.");

        if (unitPrice < 0)
            return Result<OrderItem>.Fail("UnitPrice precisa ser um valor positivo.");

        if (discount < 0)
            return Result<OrderItem>.Fail("Discount precisa ser um valor positivo.");

        return Result<OrderItem>.Ok(new OrderItem(orderId, productId, quantity, unitPrice, discount));
    }
}
