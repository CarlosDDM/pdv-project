using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class DevolutionItem
{
    public int Id { get; private set; }
    public int DevolutionId { get; private set; }
    public int OrderItemId { get; private set; }
    public int Quantity { get; private set; }
    public DevolutionItemStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private DevolutionItem() { }
}
