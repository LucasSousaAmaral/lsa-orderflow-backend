using LSA.OrderFlow.Application.Abstractions;
using LSA.OrderFlow.Domain.Abstraction;
using System.Text.Json;

namespace LSA.OrderFlow.Infrastructure.Sql.Outbox;

public sealed class OutboxSql : IOutbox
{
	private readonly OrderFlowDbContext _db;

	public OutboxSql(OrderFlowDbContext db) => _db = db;

	public Task AddAsync(OutboxEnvelope envelope, CancellationToken ct)
	{
		_db.OutboxMessages.Add(new OutboxMessage
		{
			Id = Guid.NewGuid(),
			OccurredOnUtc = envelope.OccurredOnUtc,
			Type = envelope.Type,
			Payload = envelope.Payload,
			ProcessedOnUtc = null,
			RetryCount = 0,
			NextAttemptOnUtc = null,
			Error = null
		});

		return Task.CompletedTask;
	}

	public Task AddManyAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct)
	{
		foreach (var e in domainEvents)
		{
			_db.OutboxMessages.Add(new OutboxMessage
			{
				Id = Guid.NewGuid(),
				OccurredOnUtc = e.OccurredOnUtc,
				Type = e.GetType().FullName!,
				Payload = JsonSerializer.Serialize(e),
				ProcessedOnUtc = null,
				RetryCount = 0,
				NextAttemptOnUtc = null,
				Error = null
			});
		}

		return Task.CompletedTask;
	}
}