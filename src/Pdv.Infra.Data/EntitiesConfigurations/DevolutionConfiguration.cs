using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class DevolutionConfiguration : IEntityTypeConfiguration<Devolution>
{
    public void Configure(EntityTypeBuilder<Devolution> builder)
    {
        builder.ToTable("Devolutions");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedOnAdd();

        builder.Property(d => d.OrderId)
            .IsRequired();

        builder.Property(d => d.Reason)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt);

        builder.HasIndex(d => d.OrderId)
            .HasDatabaseName("IX_Devolutions_OrderId");

        builder.HasIndex(d => new { d.OrderId, d.CreatedAt })
            .HasDatabaseName("IX_Devolutions_OrderId_CreatedAt");

        builder.HasMany(d => d.DevolutionItems)
            .WithOne()
            .HasForeignKey(di => di.DevolutionId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}
