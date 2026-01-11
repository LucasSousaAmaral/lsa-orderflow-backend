using LSA.OrderFlow.Domain.Abstraction;

namespace LSA.OrderFlow.Domain.Orders.Events;

public sealed class OrderCreatedDomainEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid CustomerId { get; }
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;

    public OrderCreatedDomainEvent(Guid orderId, Guid customerId)
    {
        OrderId = orderId; CustomerId = customerId;
    }
}

public sealed class OrderStatusChangedDomainEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public string NewStatus { get; }
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;

    public OrderStatusChangedDomainEvent(Guid orderId, string newStatus)
    {
        OrderId = orderId; NewStatus = newStatus;
    }
}

public sealed class OrderItemAddedDomainEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid ItemId { get; }
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;

    public OrderItemAddedDomainEvent(Guid orderId, Guid itemId)
    { OrderId = orderId; ItemId = itemId; }
}

public sealed class OrderItemRemovedDomainEvent : IDomainEvent
{
    public Guid OrderId { get; }
    public Guid ItemId { get; }
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;

    public OrderItemRemovedDomainEvent(Guid orderId, Guid itemId)
    { OrderId = orderId; ItemId = itemId; }
}

public sealed record OrderCancelledDomainEvent : IDomainEvent
{
	public Guid OrderId { get; }
	public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;

	public OrderCancelledDomainEvent(Guid orderId)
	{ OrderId = orderId;}
}