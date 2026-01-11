using MediatR;

namespace LSA.OrderFlow.Application.Orders.Commands.DeleteOrder;

public record DeleteOrderCommand(Guid OrderId) : IRequest<Unit>;