using LSA.OrderFlow.Domain.Customers;
using LSA.OrderFlow.Domain.Orders;
using LSA.OrderFlow.Domain.Products;
using LSA.OrderFlow.Infrastructure.Sql.Outbox;
using Microsoft.EntityFrameworkCore;

namespace LSA.OrderFlow.Infrastructure.Sql;

public class OrderFlowDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();


    public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}