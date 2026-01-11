namespace LSA.OrderFlow.Application.Orders.ViewModels;

public record OrderListItemVm(Guid Id, Guid CustomerId, DateTime OrderDate, string Status, decimal TotalAmount);
