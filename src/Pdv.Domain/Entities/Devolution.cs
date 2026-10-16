namespace Pdv.Domain.Entities;

public sealed class Devolution
{
    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public string? Reason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<DevolutionItem> _devolutionItems = [];
    public IReadOnlyCollection<DevolutionItem> DevolutionItems => _devolutionItems.AsReadOnly();

    private Devolution() { }
}
