using Pdv.Domain.Enums;

namespace Pdv.Domain.Entities;

public sealed class DevolutionItem
{
    public Guid Id { get; private set; }
    public Guid DevolutionId { get; private set; }
    public Guid OrderItemId { get; private set; }
    public int Qtd { get; private set; }
    public DevolutionItemStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private DevolutionItem() { }
}
