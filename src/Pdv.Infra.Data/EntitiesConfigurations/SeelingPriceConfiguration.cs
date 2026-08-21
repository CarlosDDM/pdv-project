using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class SeelingPriceConfiguration : IEntityTypeConfiguration<SeelingPrice>
{
    public void Configure(EntityTypeBuilder<SeelingPrice> builder)
    {
        builder.ToTable("SeelingPrices");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Id)
            .ValueGeneratedOnAdd();

        builder.Property(sp => sp.ProductId)
            .IsRequired();

        builder.Property(sp => sp.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(sp => sp.CreatedAt)
            .IsRequired();

        builder.HasIndex(sp => new { sp.ProductId, sp.CreatedAt})
            .HasDatabaseName("IX_SeelingPrices_ProductId_CreatedAt");
    }
}
