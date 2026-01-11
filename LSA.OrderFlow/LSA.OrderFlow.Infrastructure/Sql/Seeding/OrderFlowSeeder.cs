using LSA.OrderFlow.Domain.Costumers;
using LSA.OrderFlow.Domain.Customers;
using LSA.OrderFlow.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LSA.OrderFlow.Infrastructure.Sql.Seeding;

public sealed class OrderFlowSeeder
{
	private readonly OrderFlowDbContext _db;
	private readonly ILogger<OrderFlowSeeder> _logger;

	public OrderFlowSeeder(OrderFlowDbContext db, ILogger<OrderFlowSeeder> logger)
	{
		_db = db;
		_logger = logger;
	}

	public async Task SeedAsync(CancellationToken ct)
	{
		// Customer sempre fixo
		var customerExists = await _db.Customers.AnyAsync(c => c.Id == OrderFlowSeedData.CustomerId, ct);
		if (!customerExists)
		{
			var customer = Customer.Create(
				OrderFlowSeedData.CustomerId,
				name: "Default Customer",
				email: Email.Create("customer@lsa.local"),
				phone: "+55 11 99999-9999"
			);
			_db.Customers.Add(customer);
			_logger.LogInformation("Seeded default customer: {CustomerId}", OrderFlowSeedData.CustomerId);
		}

		foreach (var (id, name, unitPrice) in OrderFlowSeedData.Products)
		{
			var exists = await _db.Products.AnyAsync(p => p.Id == id, ct);
			if (exists) continue;

			var p = Product.Create(id, name, Money.Create(unitPrice));
			_db.Products.Add(p);
			_logger.LogInformation("Seeded product: {ProductId} ({Name})", id, name);
		}

		await _db.SaveChangesAsync(ct);
	}
}