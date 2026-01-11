using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LSA.OrderFlow.Infrastructure.Sql;

public sealed class OrderFlowDbContextFactory : IDesignTimeDbContextFactory<OrderFlowDbContext>
{
    public OrderFlowDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
            .UseSqlServer(@"Server=localhost\SQLEXPRESS01;Database=OrderFlowDb;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new OrderFlowDbContext(options);
    }
}