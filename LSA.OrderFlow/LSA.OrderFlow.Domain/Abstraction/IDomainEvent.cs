namespace LSA.OrderFlow.Domain.Abstraction;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}