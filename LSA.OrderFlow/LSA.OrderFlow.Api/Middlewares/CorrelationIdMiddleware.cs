using System.Diagnostics;

namespace LSA.OrderFlow.Api.Middleware;

public sealed class CorrelationIdMiddleware
{
	private const string HeaderName = "X-Correlation-Id";
	private readonly RequestDelegate _next;
	private readonly ILogger<CorrelationIdMiddleware> _logger;

	public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext ctx)
	{
		var correlationId = ctx.Request.Headers.TryGetValue(HeaderName, out var value) && !string.IsNullOrWhiteSpace(value)
			? value.ToString()
			: Guid.NewGuid().ToString("N");

		ctx.Response.Headers[HeaderName] = correlationId;
		ctx.Items[HeaderName] = correlationId;

		using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
		{
			var sw = Stopwatch.StartNew();
			await _next(ctx);
			sw.Stop();

			_logger.LogInformation("{Method} {Path} => {StatusCode} in {ElapsedMs}ms",
				ctx.Request.Method,
				ctx.Request.Path.Value,
				ctx.Response.StatusCode,
				sw.ElapsedMilliseconds);
		}
	}
}