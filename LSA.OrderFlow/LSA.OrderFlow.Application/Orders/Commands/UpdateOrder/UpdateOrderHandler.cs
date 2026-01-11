using LSA.OrderFlow.Application.Abstractions;
using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Application.Orders.Events;
using LSA.OrderFlow.Domain.Orders;
using LSA.OrderFlow.Domain.Products;
using MediatR;
using System.Text.Json;

namespace LSA.OrderFlow.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderHandler : IRequestHandler<UpdateOrderCommand, Unit>
{
	private readonly IOrderRepository _orders;
	private readonly IProductRepository _products;
	private readonly IUnitOfWork _uow;
	private readonly IOutbox _outbox;

	public UpdateOrderHandler(IOrderRepository orders, IProductRepository products, IUnitOfWork uow, IOutbox outbox)
	{
		_orders = orders;
		_products = products;
		_uow = uow;
		_outbox = outbox;
	}

	public async Task<Unit> Handle(UpdateOrderCommand cmd, CancellationToken ct)
	{
		var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
			?? throw new KeyNotFoundException("order not found");

		if (cmd.ReplaceItems is not null)
		{
			foreach (var item in order.Items.ToList())
				order.RemoveItem(item.Id);

			foreach (var i in cmd.ReplaceItems)
			{
				var product = await _products.GetByIdAsync(i.ProductId, ct)
					?? throw new KeyNotFoundException($"product not found: {i.ProductId}");

				order.AddItem(product.Id, product.Name, product.UnitPrice, i.Quantity);
			}

			var evt = new OrderItemsReplacedIntegrationEvent(
				order.Id,
				order.Items.Select(i => new OrderItemIntegrationDto(
					i.Id,
					i.ProductId,
					i.ProductName,
					i.UnitPrice.Amount,
					i.Quantity,
					i.TotalPrice.Amount
				)).ToList(),
				order.TotalAmount.Amount,
				DateTime.UtcNow
			);

			await _outbox.AddAsync(new OutboxEnvelope(
				nameof(OrderItemsReplacedIntegrationEvent),
				JsonSerializer.Serialize(evt),
				evt.OccurredOnUtc
			), ct);
		}

		if (cmd.NewStatus is int s)
		{
			order.UpdateStatus((OrderStatus)s);

			var evt = new OrderStatusChangedIntegrationEvent(order.Id, order.Status.ToString(), DateTime.UtcNow);

			await _outbox.AddAsync(new OutboxEnvelope(
				nameof(OrderStatusChangedIntegrationEvent),
				JsonSerializer.Serialize(evt),
				evt.OccurredOnUtc
			), ct);
		}

		await _uow.CommitAsync(ct);

		return Unit.Value;
	}
}