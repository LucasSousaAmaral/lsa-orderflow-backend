namespace LSA.OrderFlow.Application.Orders.Events;

public sealed record OrderCancelledIntegrationEvent(Guid OrderId, DateTime OccurredOnUtc);