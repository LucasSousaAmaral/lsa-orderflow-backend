using LSA.OrderFlow.Application.Orders.ViewModels;
using MediatR;

namespace LSA.OrderFlow.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDetailsVm>;