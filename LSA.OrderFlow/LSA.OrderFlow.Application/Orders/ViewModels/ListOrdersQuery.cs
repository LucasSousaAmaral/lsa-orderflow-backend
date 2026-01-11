using MediatR;

namespace LSA.OrderFlow.Application.Orders.ViewModels;

public record ListOrdersQuery(int Page = 1, int PageSize = 20, string? Search = null) : IRequest<PagedList<OrderListItemVm>>;