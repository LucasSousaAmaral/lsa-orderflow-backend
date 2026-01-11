using Autofac;
using LSA.OrderFlow.Application.Abstractions;
using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Infrastructure.Sql;
using LSA.OrderFlow.Infrastructure.Sql.Outbox;
using LSA.OrderFlow.Infrastructure.Sql.Repositories;

namespace LSA.OrderFlow.CrossCutting;

public class SqlModule : Module
{
	protected override void Load(ContainerBuilder b)
	{
		b.RegisterType<OrderRepositorySql>().As<IOrderRepository>().InstancePerLifetimeScope();
		b.RegisterType<ProductRepositorySql>().As<IProductRepository>().InstancePerLifetimeScope();
		b.RegisterType<UnitOfWorkSql>().As<IUnitOfWork>().InstancePerLifetimeScope();
		b.RegisterType<OutboxSql>().As<IOutbox>().InstancePerLifetimeScope();
		b.RegisterType<OutboxProcessor>().AsSelf().SingleInstance();
	}
}