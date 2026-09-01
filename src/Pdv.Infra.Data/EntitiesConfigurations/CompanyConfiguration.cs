using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>();

        builder.Property(c => c.Cnpj)
            .IsRequired()
            .HasMaxLength(18);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.HasIndex(c => c.Cnpj)
            .IsUnique()
            .HasDatabaseName("IX_Companies_Cnpj");

        builder.HasIndex(c => new { c.Name, c.ReferedTo })
            .HasDatabaseName("IX_Companies_Name_ReferedTo");

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(c => c.ReferedTo)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(c => c.Users)
            .WithOne()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(c => c.Stocks)
            .WithOne()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}