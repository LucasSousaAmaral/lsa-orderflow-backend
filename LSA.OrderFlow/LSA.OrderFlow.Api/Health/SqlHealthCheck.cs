using LSA.OrderFlow.Infrastructure.Sql;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LSA.OrderFlow.Api.Health;

public sealed class SqlHealthCheck : IHealthCheck
{
	private readonly OrderFlowDbContext _db;
	public SqlHealthCheck(OrderFlowDbContext db) => _db = db;

	public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
	{
		try
		{
			var ok = await _db.Database.CanConnectAsync(ct);
			return ok
				? HealthCheckResult.Healthy("SQL connection OK")
				: HealthCheckResult.Unhealthy("SQL connection FAILED");
		}
		catch (Exception ex)
		{
			return HealthCheckResult.Unhealthy("SQL health check exception", ex);
		}
	}
}