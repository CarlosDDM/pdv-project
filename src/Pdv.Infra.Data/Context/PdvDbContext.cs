using Microsoft.EntityFrameworkCore;

namespace Pdv.Infra.Data.Context;

public sealed class PdvDbContext: DbContext
{
    public PdvDbContext(DbContextOptions<PdvDbContext> options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PdvDbContext).Assembly);
    }
}
