using LSA.OrderFlow.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LSA.OrderFlow.Infrastructure.Sql.Config.Orders;

public class OrderItemConfig : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("OrderItems");
        b.HasKey(x => x.Id);
        b.Property<Guid>("OrderId").IsRequired();
        b.Property(x => x.ProductId).IsRequired();
        b.Property(x => x.ProductName).HasMaxLength(180).IsRequired();
        b.OwnsOne(x => x.UnitPrice, money =>
        {
            money.Property(p => p.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
            money.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(4).HasDefaultValue("BRL");
        });
        b.OwnsOne(x => x.Quantity, q =>
        {
            q.Property(p => p.Value).HasColumnName("Quantity");
        });
    }
}