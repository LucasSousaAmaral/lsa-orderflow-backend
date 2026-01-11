using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LSA.OrderFlow.Api.Health;

public sealed class MongoHealthCheck : IHealthCheck
{
	private readonly IMongoDatabase _db;
	public MongoHealthCheck(IMongoDatabase db) => _db = db;

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
	{
		try
		{
			await _db.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: ct);
			return HealthCheckResult.Healthy("Mongo connection OK");
		}
		catch (Exception ex)
		{
			return HealthCheckResult.Unhealthy("Mongo health check exception", ex);
		}
	}
}