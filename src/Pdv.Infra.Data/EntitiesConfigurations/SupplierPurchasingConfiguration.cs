using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class SupplierPurchasingConfiguration : IEntityTypeConfiguration<SupplierPurchasing>
{
    public void Configure(EntityTypeBuilder<SupplierPurchasing> builder)
    {
        builder.ToTable("SuppliersPurchasing");

        builder.HasKey(sp => sp.Id);
        builder.Property(sp => sp.Id)
            .ValueGeneratedOnAdd();

        builder.Property(sp => sp.SupplierId)
            .IsRequired();

        builder.Property(sp => sp.ProductId)
            .IsRequired();

        builder.Property(sp => sp.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(sp => sp.CreatedAt)
            .IsRequired();

        builder.HasIndex(sp => sp.SupplierId)
            .HasDatabaseName("IX_SuppliersPurchasing_SupplierId");

        builder.HasIndex(sp => new { sp.ProductId, sp.CreatedAt })
            .HasDatabaseName("IX_SuppliersPurchasing_ProductId_CreatedAt");
    }
}
