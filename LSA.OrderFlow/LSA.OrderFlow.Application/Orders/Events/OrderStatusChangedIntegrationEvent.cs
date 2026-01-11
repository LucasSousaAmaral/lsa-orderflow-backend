namespace LSA.OrderFlow.Application.Orders.Events;

public sealed record OrderStatusChangedIntegrationEvent(Guid OrderId, string NewStatus, DateTime OccurredOnUtc);