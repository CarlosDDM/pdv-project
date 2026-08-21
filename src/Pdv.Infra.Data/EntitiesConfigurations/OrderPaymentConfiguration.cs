using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pdv.Domain.Entities;

namespace Pdv.Infra.Data.EntitiesConfigurations;

public sealed class OrderPaymentConfiguration : IEntityTypeConfiguration<OrderPayment>
{
    public void Configure(EntityTypeBuilder<OrderPayment> builder)
    {
        builder.ToTable("OrderPayments");

        builder.HasKey(op => op.Id);

        builder.Property(op => op.Id)
            .ValueGeneratedOnAdd();

        builder.Property(op => op.OrderId)
            .IsRequired();

        builder.Property(op => op.PaymentMethod)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(op => op.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(op => op.CreatedAt)
            .IsRequired();

        builder.HasIndex(op => op.OrderId)
            .HasDatabaseName("IX_OrderPayments_OrderId");

        builder.HasIndex(op => new { op.PaymentMethod, op.CreatedAt })
            .HasDatabaseName("IX_OrderPayments_PaymentMethod_CreatedAt");
    }
}
