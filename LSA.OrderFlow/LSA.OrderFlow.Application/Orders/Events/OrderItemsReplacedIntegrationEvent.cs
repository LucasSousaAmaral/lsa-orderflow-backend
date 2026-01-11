namespace LSA.OrderFlow.Application.Orders.Events;

public sealed record OrderItemsReplacedIntegrationEvent(Guid OrderId, List<OrderItemIntegrationDto> Items, decimal TotalAmount, DateTime OccurredOnUtc);
