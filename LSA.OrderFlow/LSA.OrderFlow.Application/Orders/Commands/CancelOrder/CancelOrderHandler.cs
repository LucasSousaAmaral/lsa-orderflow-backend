using LSA.OrderFlow.Application.Abstractions;
using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Application.Orders.Events;
using MediatR;
using System.Text.Json;

namespace LSA.OrderFlow.Application.Orders.Commands.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, Unit>
{
	private readonly IOrderRepository _orders;
	private readonly IOutbox _outbox;
	private readonly IUnitOfWork _uow;

	public CancelOrderHandler(
		IOrderRepository orders,
		IOutbox outbox,
		IUnitOfWork uow)
	{
		_orders = orders;
		_outbox = outbox;
		_uow = uow;
	}

	public async Task<Unit> Handle(CancelOrderCommand cmd, CancellationToken ct)
	{
		var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
			?? throw new KeyNotFoundException("order not found");

		order.Cancel();

		var evt = new OrderCancelledIntegrationEvent(order.Id, DateTime.UtcNow);

		await _outbox.AddAsync(new OutboxEnvelope(
			nameof(OrderCancelledIntegrationEvent),
			JsonSerializer.Serialize(evt),
			evt.OccurredOnUtc
		), ct);

		await _uow.CommitAsync(ct);
		return Unit.Value;
	}
}