using LSA.OrderFlow.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LSA.OrderFlow.Infrastructure.Sql.Config.Products;

public class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(180).IsRequired();
        b.OwnsOne(x => x.UnitPrice, money =>
        {
            money.Property(p => p.Amount).HasColumnName("UnitPrice").HasPrecision(18, 2);
            money.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(4).HasDefaultValue("BRL");
        });
    }
}