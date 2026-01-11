namespace LSA.OrderFlow.Infrastructure.Sql.Seeding;

public static class OrderFlowSeedData
{
	public static readonly Guid CustomerId = Guid.Parse("11111111-1111-1111-1111-111111111111");

	// 3 produtos fixos
	public static readonly (Guid Id, string Name, decimal UnitPrice)[] Products =
	[
		(Guid.Parse("22222222-2222-2222-2222-222222222221"), "Keyboard", 199.90m),
	(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Mouse", 79.90m),
	(Guid.Parse("22222222-2222-2222-2222-222222222223"), "Headset", 249.90m)
	];
}