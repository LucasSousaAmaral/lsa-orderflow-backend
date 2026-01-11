using LSA.OrderFlow.Domain.Abstraction;
using LSA.OrderFlow.Domain.Orders.Events;
using LSA.OrderFlow.Domain.Products;
using LSA.OrderFlow.Domain.Shared;

namespace LSA.OrderFlow.Domain.Orders;

public sealed class Order : Entity
{
    public Guid Id { get; }
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public Money TotalAmount { get; private set; } = Money.Zero;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    //Para o EF
    private Order() { }

    private Order(Guid id, Guid customerId, DateTime orderDateUtc)
    {
        Id = id;
        CustomerId = customerId;
        OrderDate = orderDateUtc;
    }

    public static Order Create(Guid customerId, DateTime nowUtc)
    {
        Guard.Against(nameof(customerId), customerId == Guid.Empty, "invalid customer id");
        var order = new Order(Guid.NewGuid(), customerId, nowUtc);
        order.Raise(new OrderCreatedDomainEvent(order.Id, customerId));
        return order;
    }

    public OrderItem AddItem(Guid productId, string productName, Money unitPrice, int qty)
    {
        Guard.Against(nameof(productId), productId == Guid.Empty, "invalid product id");
        Guard.Against(nameof(productName), string.IsNullOrWhiteSpace(productName), "empty name");

        var item = OrderItem.Create(productId, productName.Trim(), unitPrice, qty);
        _items.Add(item);
        RecalculateTotal();
        Raise(new OrderItemAddedDomainEvent(Id, item.Id));
        return item;
    }

    public void RemoveItem(Guid itemId)
    {
        var removed = _items.RemoveAll(i => i.Id == itemId);
        Guard.Against(nameof(itemId), removed == 0, "item not found");
        RecalculateTotal();
        Raise(new OrderItemRemovedDomainEvent(Id, itemId));
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Guard.Against(nameof(Status), Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled,
            "cannot change a canceled order");
        if (newStatus == Status) return;

        Status = newStatus;
        Raise(new OrderStatusChangedDomainEvent(Id, newStatus.ToString()));
    }

    private void RecalculateTotal() =>
        TotalAmount = _items.Aggregate(Money.Zero, (acc, i) => acc + i.TotalPrice);

	public void Cancel()
	{
		if (Status == OrderStatus.Cancelled)
			return;

		Status = OrderStatus.Cancelled;
		Raise(new OrderCancelledDomainEvent(Id));
	}
}
