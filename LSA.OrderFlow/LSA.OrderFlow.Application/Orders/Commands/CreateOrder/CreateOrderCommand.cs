using LSA.OrderFlow.Application.Orders.Dtos;
using MediatR;

namespace LSA.OrderFlow.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(Guid CustomerId, IReadOnlyList<CreateOrderItemDto> Items) : IRequest<Guid>;