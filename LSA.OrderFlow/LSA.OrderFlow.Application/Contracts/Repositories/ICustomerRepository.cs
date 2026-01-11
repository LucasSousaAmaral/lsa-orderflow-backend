using LSA.OrderFlow.Domain.Customers;

namespace LSA.OrderFlow.Application.Contracts.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct);
}
