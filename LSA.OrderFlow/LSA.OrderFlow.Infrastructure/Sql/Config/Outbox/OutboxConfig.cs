using LSA.OrderFlow.Infrastructure.Sql.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LSA.OrderFlow.Infrastructure.Sql.Config.Outbox;

public class OutboxConfig : IEntityTypeConfiguration<OutboxMessage>
{
	public void Configure(EntityTypeBuilder<OutboxMessage> b)
	{
		b.ToTable("OutboxMessages");
		b.HasKey(x => x.Id);
		b.Property(x => x.Type).HasMaxLength(240).IsRequired();
		b.Property(x => x.Payload).IsRequired();
		b.Property(x => x.OccurredOnUtc).IsRequired();

		b.Property(x => x.RetryCount).HasDefaultValue(0);
		b.Property(x => x.NextAttemptOnUtc);
		b.Property(x => x.Error).HasMaxLength(2000);
	}
}