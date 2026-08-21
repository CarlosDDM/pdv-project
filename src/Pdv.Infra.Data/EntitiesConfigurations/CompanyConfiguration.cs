using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companys");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Type)
            .HasConversion<string>();

        builder.Property(c => c.Cnpj);

        builder.Property(c => c.ReferedTo);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.HasIndex(c => new { c.Name, c.ReferedTo })
            .HasDatabaseName("IX_Companys_Name_ReferedTo");

        builder.HasMany(c => c.Users)
            .WithOne()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);

    }
}
