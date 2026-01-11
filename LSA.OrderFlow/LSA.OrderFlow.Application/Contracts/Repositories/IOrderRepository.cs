using LSA.OrderFlow.Domain.Orders;
namespace LSA.OrderFlow.Application.Contracts.Repositories;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task RemoveAsync(Order order, CancellationToken ct);
}
