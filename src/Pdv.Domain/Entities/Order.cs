using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<OrderItem> _orderItems = [];
    private readonly List<OrderPayment> _orderPayments = [];
    private readonly List<Devolution> _devolutions = [];

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    public IReadOnlyCollection<OrderPayment> OrderPayments => _orderPayments.AsReadOnly();
    public IReadOnlyCollection<Devolution> Devolutions => _devolutions.AsReadOnly();

    private Order() { }
}
