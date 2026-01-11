using LSA.OrderFlow.Domain.Abstraction;

namespace LSA.OrderFlow.Application.Orders.Events;

public sealed record OrderCancelledDomainEvent(Guid OrderId) : IDomainEvent
{
	public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}