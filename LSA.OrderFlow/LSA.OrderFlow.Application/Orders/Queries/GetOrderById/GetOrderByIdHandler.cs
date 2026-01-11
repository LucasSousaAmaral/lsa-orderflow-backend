using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Application.Orders.ViewModels;
using MediatR;

namespace LSA.OrderFlow.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDetailsVm>
{
    private readonly IOrderReadRepository _read;
    public GetOrderByIdHandler(IOrderReadRepository read) => _read = read;

    public async Task<OrderDetailsVm> Handle(GetOrderByIdQuery q, CancellationToken ct)
        => await _read.GetDetailsAsync(q.OrderId, ct)
            ?? throw new KeyNotFoundException("order not found");
}