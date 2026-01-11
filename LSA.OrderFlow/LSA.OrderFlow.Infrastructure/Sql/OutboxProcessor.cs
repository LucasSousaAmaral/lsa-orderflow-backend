using LSA.OrderFlow.Infrastructure.Mongo.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LSA.OrderFlow.Infrastructure.Sql;

public class OutboxProcessor : BackgroundService
{
	private const int BatchSize = 50;
	private const int MaxRetries = 10;
	private static readonly TimeSpan IdleDelay = TimeSpan.FromSeconds(2);

	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<OutboxProcessor> _logger;

	public OutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessor> logger)
	{
		_scopeFactory = scopeFactory;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			try
			{
				using var scope = _scopeFactory.CreateScope();

				var db = scope.ServiceProvider.GetRequiredService<OrderFlowDbContext>();
				var dispatcher = scope.ServiceProvider.GetRequiredService<IProjectionDispatcher>();

				var now = DateTime.UtcNow;
				var messages = await db.OutboxMessages
					.Where(m => m.ProcessedOnUtc == null && (m.NextAttemptOnUtc == null || m.NextAttemptOnUtc <= now))
					.OrderBy(m => m.OccurredOnUtc)
					.Take(BatchSize)
					.ToListAsync(stoppingToken);

				if (messages.Count == 0)
				{
					await Task.Delay(IdleDelay, stoppingToken);
					continue;
				}

				foreach (var m in messages)
				{
					try
					{
						await dispatcher.DispatchAsync(m.Type, m.Payload, stoppingToken);
						m.ProcessedOnUtc = now;
						m.Error = null;
						m.NextAttemptOnUtc = null;
					}
					catch (Exception ex)
					{
						m.RetryCount++;
						m.Error = TrimTo(ex.ToString(), 2000);

						if (m.RetryCount >= MaxRetries)
						{
							m.ProcessedOnUtc = now;
							m.NextAttemptOnUtc = null;
							_logger.LogError(ex, "Outbox {Id} exceeded max retries ({Retries}). Marked as processed.", m.Id, m.RetryCount);
						}
						else
						{
							var backoff = ComputeBackoffSeconds(m.RetryCount);
							m.NextAttemptOnUtc = now.AddSeconds(backoff);
							_logger.LogError(ex, "Failed to dispatch outbox {Id}. Retry={Retry} NextAttemptIn={Backoff}s", m.Id, m.RetryCount, backoff);
						}
					}
				}

				await db.SaveChangesAsync(stoppingToken);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "OutboxProcessor loop failed");
			}
		}
	}

	//Calcular tempo de espera
	private static int ComputeBackoffSeconds(int retryCount)
	{
		var pow = (int)Math.Pow(2, Math.Clamp(retryCount, 1, 5));
		var seconds = 2 * pow;
		return Math.Min(seconds, 60);
	}

	private static string TrimTo(string value, int maxLen)
		=> value.Length <= maxLen ? value : value[..maxLen];
}
