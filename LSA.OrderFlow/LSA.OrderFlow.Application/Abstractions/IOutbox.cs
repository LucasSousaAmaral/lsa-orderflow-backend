using LSA.OrderFlow.Domain.Abstraction;

namespace LSA.OrderFlow.Application.Abstractions;

public interface IOutbox
{
    Task AddAsync(OutboxEnvelope envelope, CancellationToken ct);
    Task AddManyAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct);
}