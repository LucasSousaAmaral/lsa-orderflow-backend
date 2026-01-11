using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace LSA.OrderFlow.Infrastructure.Sql.Repositories;

public class OrderRepositorySql : IOrderRepository
{
    private readonly OrderFlowDbContext _db;
    public OrderRepositorySql(OrderFlowDbContext db) => _db = db;


    public async Task AddAsync(Order order, CancellationToken ct)
    {
        await _db.Orders.AddAsync(order, ct);
    }


    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct)
    => _db.Orders.Include("_items").FirstOrDefaultAsync(x => x.Id == id, ct);


    public Task RemoveAsync(Order order, CancellationToken ct)
    {
        _db.Orders.Remove(order);
        return Task.CompletedTask;
    }
}