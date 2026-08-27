using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);
        builder.Property(oi => oi.Id)
            .ValueGeneratedOnAdd();

        builder.Property(oi => oi.OrderId)
            .IsRequired();
        
        builder.Property(oi => oi.ProductId)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Property(oi => oi.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(oi => oi.Discount)
            .HasPrecision(18, 2);

        builder.Property(oi => oi.CreatedAt)
            .IsRequired();

        builder.HasIndex(oi => oi.ProductId)             
            .HasDatabaseName("IX_OrderItems_ProductId");

        builder.HasIndex(oi => oi.OrderId)
            .HasDatabaseName("IX_OrderItems_OrderId");

        builder.HasMany(oi => oi.DevolutionItems)
            .WithOne()
            .HasForeignKey(di => di.OrderItemId)
            .OnDelete(DeleteBehavior.NoAction);
    }

}
