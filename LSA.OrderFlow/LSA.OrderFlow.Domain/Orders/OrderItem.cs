using LSA.OrderFlow.Domain.Abstraction;
using LSA.OrderFlow.Domain.Products;

namespace LSA.OrderFlow.Domain.Orders;

public sealed class OrderItem : Entity
{
    public Guid Id { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public Money UnitPrice { get; }
    public Quantity Quantity { get; }
    public Money TotalPrice => UnitPrice * (int)Quantity;

    //Para o EF
    private OrderItem() { }

    private OrderItem(Guid id, Guid productId, string name, Money price, Quantity qty)
    {
        Id = id;
        ProductId = productId;
        ProductName = name;
        UnitPrice = price;
        Quantity = qty;
    }

    public static OrderItem Create(Guid productId, string productName, Money unitPrice, int qty) =>
        new(Guid.NewGuid(), productId, productName, unitPrice, Quantity.Create(qty));
}
