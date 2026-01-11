using LSA.OrderFlow.Application.Abstractions;
using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Application.Orders.Events;
using LSA.OrderFlow.Domain.Orders;
using LSA.OrderFlow.Domain.Products;
using MediatR;
using System.Text.Json;

namespace LSA.OrderFlow.Application.Orders.Commands.CreateOrder;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
{
	private readonly IOrderRepository _orders;
	private readonly IProductRepository _products;
	private readonly IUnitOfWork _uow;
	private readonly IOutbox _outbox;

	public CreateOrderHandler(IOrderRepository orders, IProductRepository products, IUnitOfWork uow, IOutbox outbox)
	{
		_orders = orders;
		_products = products;
		_uow = uow;
		_outbox = outbox;
	}

	public async Task<Guid> Handle(CreateOrderCommand cmd, CancellationToken ct)
	{
		var order = Order.Create(cmd.CustomerId, DateTime.UtcNow);

		foreach (var i in cmd.Items)
		{
			var product = await _products.GetByIdAsync(i.ProductId, ct)
				?? throw new KeyNotFoundException($"product not found: {i.ProductId}");

			// Snapshot do catálogo no momento do pedido
			order.AddItem(product.Id, product.Name, product.UnitPrice, i.Quantity);
		}

		await _orders.AddAsync(order, ct);

		var evt = new OrderCreatedIntegrationEvent(
			order.Id,
			order.CustomerId,
			order.OrderDate,
			order.Status.ToString(),
			order.TotalAmount.Amount,
			order.Items.Select(i => new OrderItemIntegrationDto(
				i.Id,
				i.ProductId,
				i.ProductName,
				i.UnitPrice.Amount,
				i.Quantity,
				i.TotalPrice.Amount
			)).ToList()
		);

		var payload = JsonSerializer.Serialize(evt);

		await _outbox.AddAsync(
			new OutboxEnvelope(nameof(OrderCreatedIntegrationEvent), payload, DateTime.UtcNow),
			ct
		);

		await _uow.CommitAsync(ct);

		return order.Id;
	}

}