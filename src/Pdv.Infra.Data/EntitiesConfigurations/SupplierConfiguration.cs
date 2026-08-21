using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Cnpj);

        builder.Property(s => s.Phone);

        builder.Property(s => s.Email);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.HasIndex(s => s.Cnpj)
            .HasDatabaseName("Suppliers_Cnpj");

        builder.HasIndex(s => s.Name)
            .HasDatabaseName("Suppliers_Name");

        builder.HasMany(s => s.SupplierPurchasings)
            .WithOne()
            .HasForeignKey(sp => sp.SupplierId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
