namespace LSA.OrderFlow.Infrastructure.Mongo.Projections;

public interface IProjectionDispatcher
{
    Task DispatchAsync(string type, string payload, CancellationToken ct);
}
