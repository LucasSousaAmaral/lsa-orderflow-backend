using MediatR;

namespace LSA.OrderFlow.Application.Orders.Commands.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId) : IRequest<Unit>;