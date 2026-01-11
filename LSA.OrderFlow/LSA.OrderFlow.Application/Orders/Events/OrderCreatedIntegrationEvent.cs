namespace LSA.OrderFlow.Application.Orders.Events;

public sealed record OrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    IReadOnlyList<OrderItemIntegrationDto> Items
);

public sealed record OrderItemIntegrationDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice
);