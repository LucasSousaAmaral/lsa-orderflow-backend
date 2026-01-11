namespace LSA.OrderFlow.Application.Orders.Dtos
{
	public record CreateOrderItemDto(Guid ProductId, int Quantity);
}
