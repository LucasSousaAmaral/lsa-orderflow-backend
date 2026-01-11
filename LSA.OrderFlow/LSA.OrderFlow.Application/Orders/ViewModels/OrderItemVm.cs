namespace LSA.OrderFlow.Application.Orders.ViewModels;

public record OrderItemVm(Guid Id, Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal TotalPrice);