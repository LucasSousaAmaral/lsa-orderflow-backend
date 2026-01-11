using LSA.OrderFlow.Application.Abstractions;

namespace LSA.OrderFlow.Infrastructure.Sql;

public class UnitOfWorkSql : IUnitOfWork
{
    private readonly OrderFlowDbContext _db;
    public UnitOfWorkSql(OrderFlowDbContext db) => _db = db;
    public Task<int> CommitAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}