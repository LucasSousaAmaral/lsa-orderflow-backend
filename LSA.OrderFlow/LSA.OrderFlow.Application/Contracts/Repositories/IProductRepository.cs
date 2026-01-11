using LSA.OrderFlow.Domain.Products;

namespace LSA.OrderFlow.Application.Contracts.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
}