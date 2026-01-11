namespace LSA.OrderFlow.Application.Orders.ViewModels;

public record OrderDetailsVm(Guid Id, Guid CustomerId, DateTime OrderDate, string Status, decimal TotalAmount, IReadOnlyList<OrderItemVm> Items);