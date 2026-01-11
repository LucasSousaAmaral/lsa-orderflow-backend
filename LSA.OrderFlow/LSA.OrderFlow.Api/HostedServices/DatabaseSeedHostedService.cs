using LSA.OrderFlow.Infrastructure.Sql.Seeding;

namespace LSA.OrderFlow.Api.HostedServices;

public sealed class DatabaseSeedHostedService : IHostedService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<DatabaseSeedHostedService> _logger;

	public DatabaseSeedHostedService(IServiceScopeFactory scopeFactory, ILogger<DatabaseSeedHostedService> logger)
	{
		_scopeFactory = scopeFactory;
		_logger = logger;
	}

	public async Task StartAsync(CancellationToken ct)
	{
		try
		{
			using var scope = _scopeFactory.CreateScope();
			var seeder = scope.ServiceProvider.GetRequiredService<OrderFlowSeeder>();
			await seeder.SeedAsync(ct);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Database seeding failed");
		}
	}

	public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}