namespace LSA.OrderFlow.Application.Orders.Events;

public sealed record OrderDeletedIntegrationEvent(Guid OrderId, DateTime OccurredOnUtc);