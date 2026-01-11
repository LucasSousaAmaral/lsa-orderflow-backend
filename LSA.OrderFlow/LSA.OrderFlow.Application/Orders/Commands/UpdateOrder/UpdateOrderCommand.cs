using LSA.OrderFlow.Application.Orders.Dtos;
using MediatR;

namespace LSA.OrderFlow.Application.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand(Guid OrderId, int? NewStatus, IReadOnlyList<CreateOrderItemDto>? ReplaceItems) : IRequest<Unit>;