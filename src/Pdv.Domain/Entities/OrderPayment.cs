using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class OrderPayment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public OrderPaymentMethod PaymentMethod { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private OrderPayment() { }
}
