using LSA.OrderFlow.Domain.Customers;
using LSA.OrderFlow.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LSA.OrderFlow.Infrastructure.Sql.Config.Orders;

public class OrderConfig : IEntityTypeConfiguration<Order>
{
	public void Configure(EntityTypeBuilder<Order> b)
	{
		b.ToTable("Orders");
		b.HasKey(x => x.Id);

		b.Property(x => x.CustomerId).IsRequired();
		b.HasOne<Customer>()
			.WithMany()
			.HasForeignKey(x => x.CustomerId)
			.OnDelete(DeleteBehavior.Restrict);

		b.Property(x => x.OrderDate).IsRequired();
		b.Property(x => x.Status).HasConversion<int>().IsRequired();

		b.OwnsOne(x => x.TotalAmount, money =>
		{
			money.Property(p => p.Amount).HasColumnName("TotalAmount").HasPrecision(18, 2);
			money.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(4).HasDefaultValue("BRL");
		});

		b.Ignore(x => x.Items);

		b.HasMany<OrderItem>("_items")
			.WithOne()
			.HasForeignKey("OrderId")
			.OnDelete(DeleteBehavior.Cascade);

		b.Navigation("_items").UsePropertyAccessMode(PropertyAccessMode.Field);
	}
}