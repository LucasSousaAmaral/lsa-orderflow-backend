using LSA.OrderFlow.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LSA.OrderFlow.Infrastructure.Sql.Config.Customers
{
    public class CustomerConfig : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> b)
        {
            b.ToTable("Customers");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(180).IsRequired();
            b.OwnsOne(x => x.Email, e =>
            {
                e.Property(p => p.Value).HasColumnName("Email").HasMaxLength(180).IsRequired();
            });

            b.Property(x => x.Phone).HasMaxLength(40);
        }
    }
}
