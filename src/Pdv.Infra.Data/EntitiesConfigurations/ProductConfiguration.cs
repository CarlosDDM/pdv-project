using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.Barcode)
            .HasMaxLength(20);

        builder.Property(p => p.Brand)
            .HasMaxLength(50);

        builder.Property(p => p.Type)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(200);

        builder.Property(p => p.CreatedAt)
            .IsRequired();
        
        builder.Property(p => p.UpdatedAt);

        builder.HasIndex(p => p.Barcode)
            .IsUnique()
            .HasDatabaseName("IX_Products_Barcode");

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("IX_Products_Name");

        builder.HasMany(p => p.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.SeelingPrices)
            .WithOne()
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(p => p.SuppliersPurchasing)
            .WithOne()
            .HasForeignKey(sp => sp.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
