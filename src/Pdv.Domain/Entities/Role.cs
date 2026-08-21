namespace Pdv.Domain.Entities;

public sealed class Role
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }

    private readonly List<User> _users = [];

    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Role() { }
}
