using Microsoft.EntityFrameworkCore;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.Context;

public sealed class PdvDbContext: DbContext
{
    public PdvDbContext(DbContextOptions<PdvDbContext> options) : base(options)
    {}

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PdvDbContext).Assembly);
    }
}
