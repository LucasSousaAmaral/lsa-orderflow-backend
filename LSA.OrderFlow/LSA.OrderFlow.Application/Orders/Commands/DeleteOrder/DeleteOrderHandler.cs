using LSA.OrderFlow.Application.Abstractions;
using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Application.Orders.Events;
using MediatR;
using System.Text.Json;

namespace LSA.OrderFlow.Application.Orders.Commands.DeleteOrder;

public class DeleteOrderHandler : IRequestHandler<DeleteOrderCommand, Unit>
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
	private readonly IOutbox _outbox;

	public DeleteOrderHandler(IOrderRepository orders, IUnitOfWork uow, IOutbox outbox)
    { 
        _orders = orders; 
        _uow = uow;
		_outbox = outbox;
	}

    public async Task<Unit> Handle(DeleteOrderCommand cmd, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(cmd.OrderId, ct)
            ?? throw new KeyNotFoundException("order not found");

        await _orders.RemoveAsync(order, ct);

		var evt = new OrderDeletedIntegrationEvent(order.Id, DateTime.UtcNow);

		await _outbox.AddAsync(new OutboxEnvelope(
			nameof(OrderDeletedIntegrationEvent),
			JsonSerializer.Serialize(evt),
			evt.OccurredOnUtc

		), ct);

		await _uow.CommitAsync(ct);

        return Unit.Value;
    }
}
