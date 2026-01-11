namespace LSA.OrderFlow.Application.Abstractions;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken ct = default);
}
