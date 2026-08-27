using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class StockConfiguration : IEntityTypeConfiguration<Stock>
{
    public void Configure(EntityTypeBuilder<Stock> builder)
    {
        builder.ToTable("Stocks");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.CompanyId)
            .IsRequired();

        builder.Property(s => s.Quantity)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.HasIndex(s => s.CreatedAt)
            .HasDatabaseName("IX_Stocks_CreatedAt");

        builder.HasIndex(s => s.CompanyId)
            .HasDatabaseName("IX_Stocks_CompanyId");

        builder.HasIndex(s => s.UpdatedAt)
            .HasDatabaseName("IX_Stocks_UpdatedAt");

    }
}
