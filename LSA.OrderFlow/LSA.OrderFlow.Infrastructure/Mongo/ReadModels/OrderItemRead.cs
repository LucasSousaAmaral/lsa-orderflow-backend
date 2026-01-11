namespace LSA.OrderFlow.Infrastructure.Mongo.ReadModels;

public class OrderItemRead
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

public class OrderRead
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = default!;
    public decimal TotalAmount { get; set; }
    public List<OrderItemRead> Items { get; set; } = new();
}