using LSA.OrderFlow.Application.Orders.ViewModels;

namespace LSA.OrderFlow.Application.Contracts.Repositories;

public interface IOrderReadRepository
{
    Task<OrderDetailsVm?> GetDetailsAsync(Guid orderId, CancellationToken ct);
    Task<PagedList<OrderListItemVm>> ListAsync(int page, int pageSize, string? search, CancellationToken ct);
}
