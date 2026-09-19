using Pdv.Domain.Common;
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

    private Order(Guid userId, decimal amount, OrderStatus status)
    {
        this.UserId = userId;
        this.Amount = amount;
        this.Status = status;
        this.CreatedAt = DateTime.UtcNow;
    }

    public static Result<Order> Create(Guid userId, decimal amount, OrderStatus status)
    {
        if (userId == Guid.Empty)
            return Result<Order>.Fail("UserId não pode ser vazio.");

        if (amount < 0)
            return Result<Order>.Fail("Amount precisa ser um valor positivo.");

        if (!Enum.IsDefined<OrderStatus>(status))
            return Result<Order>.Fail($"Esse estado não é valido: {status}");

        return Result<Order>.Ok(new Order(userId, amount, status));
    }
}
