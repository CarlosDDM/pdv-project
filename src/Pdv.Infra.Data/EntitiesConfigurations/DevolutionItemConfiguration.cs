using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class DevolutionItemConfiguration : IEntityTypeConfiguration<DevolutionItem>
{
    public void Configure(EntityTypeBuilder<DevolutionItem> builder)
    {
        builder.ToTable("DevolutionItems");

        builder.HasKey(di => di.Id);

        builder.Property(di => di.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(di => di.DevolutionId)
            .IsRequired();

        builder.Property(di => di.OrderItemId)
            .IsRequired();

        builder.Property(di => di.Qtd)
            .IsRequired();

        builder.Property(di => di.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(di => di.CreatedAt)
            .IsRequired();

        builder.HasIndex(di => di.DevolutionId)
            .HasDatabaseName("IX_DevolutionItems_DevolutionId");

        builder.HasIndex(di => di.OrderItemId)
            .HasDatabaseName("IX_DevolutionItems_OrderItemId");

        builder.HasIndex(di => new { di.Status, di.CreatedAt });
    }
}
