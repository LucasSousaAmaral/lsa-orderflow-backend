using System.Net;

namespace LSA.OrderFlow.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next(context);
		}
		catch (KeyNotFoundException ex)
		{
			context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			await context.Response.WriteAsJsonAsync(new { error = ex.Message });
		}
		catch (ArgumentException ex)
		{
			context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			await context.Response.WriteAsJsonAsync(new { error = ex.Message });
		}
	}
}