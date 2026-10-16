namespace Pdv.Domain.Entities;

public sealed class User
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string HashedPassword { get; private set; }
    public int CompanyId { get; private set; }
    public int RoleId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User() { }
}
