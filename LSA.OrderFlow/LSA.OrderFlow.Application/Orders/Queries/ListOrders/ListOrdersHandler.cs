using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Application.Orders.ViewModels;
using MediatR;

namespace LSA.OrderFlow.Application.Orders.Queries.ListOrders;

public class ListOrdersHandler : IRequestHandler<ListOrdersQuery, PagedList<OrderListItemVm>>
{
    private readonly IOrderReadRepository _read;
    public ListOrdersHandler(IOrderReadRepository read) => _read = read;

    public Task<PagedList<OrderListItemVm>> Handle(ListOrdersQuery q, CancellationToken ct) =>
        _read.ListAsync(q.Page, q.PageSize, q.Search, ct);
}