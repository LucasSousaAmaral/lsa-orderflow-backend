using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace LSA.OrderFlow.Infrastructure.Sql.Repositories;

public sealed class ProductRepositorySql : IProductRepository
{
	private readonly OrderFlowDbContext _db;
	public ProductRepositorySql(OrderFlowDbContext db) => _db = db;

	public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
		=> _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
}
