using LSA.OrderFlow.Application.Orders.Events;
using LSA.OrderFlow.Infrastructure.Mongo.ReadModels;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text.Json;

namespace LSA.OrderFlow.Infrastructure.Mongo.Projections;

public class ProjectionDispatcher : IProjectionDispatcher
{
    private readonly IMongoCollection<OrderRead> _orders;
    private readonly ILogger<ProjectionDispatcher> _logger;


    public ProjectionDispatcher(IMongoDatabase db, ILogger<ProjectionDispatcher> logger)
    {
        _orders = db.GetCollection<OrderRead>("orders_read");
        _logger = logger;
    }

    public async Task DispatchAsync(string type, string payload, CancellationToken ct)
    {
        switch (type)
        {
            case var t when t == nameof(OrderItemsReplacedIntegrationEvent):
                {
                    var e = JsonSerializer.Deserialize<OrderItemsReplacedIntegrationEvent>(payload)!;

                    var newItems = e.Items.Select(i => new OrderItemRead
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        UnitPrice = i.UnitPrice,
                        Quantity = i.Quantity,
                        TotalPrice = i.TotalPrice
                    }).ToList();

                    var update = Builders<OrderRead>.Update.Combine(
                        Builders<OrderRead>.Update.Set(x => x.Items, newItems),
                        Builders<OrderRead>.Update.Set(x => x.TotalAmount, e.TotalAmount)
                    );

                    var result = await _orders.UpdateOneAsync(x => x.Id == e.OrderId, update, cancellationToken: ct);

                    if (result.MatchedCount == 0)
                        _logger.LogWarning("OrderRead not found for items replace. OrderId={OrderId}", e.OrderId);

                    break;
                }
            case var t when t == nameof(OrderCreatedIntegrationEvent):
                {
                    var e = JsonSerializer.Deserialize<OrderCreatedIntegrationEvent>(payload)!;

                    var doc = new OrderRead
                    {
                        Id = e.OrderId,
                        CustomerId = e.CustomerId,
                        OrderDate = e.OrderDate,
                        Status = e.Status,
                        TotalAmount = e.TotalAmount,
                        Items = e.Items.Select(i => new OrderItemRead
                        {
                            Id = i.Id,
                            ProductId = i.ProductId,
                            ProductName = i.ProductName,
                            UnitPrice = i.UnitPrice,
                            Quantity = i.Quantity,
                            TotalPrice = i.TotalPrice
                        }).ToList()
                    };

                    await _orders.ReplaceOneAsync(
                        x => x.Id == doc.Id,
                        doc,
                        new ReplaceOptions { IsUpsert = true },
                        ct
                    );

                    break;
                }
            case var t when t == nameof(OrderStatusChangedIntegrationEvent):
                {
                    var e = JsonSerializer.Deserialize<OrderStatusChangedIntegrationEvent>(payload)!;

                    var update = Builders<OrderRead>.Update.Set(x => x.Status, e.NewStatus);
                    var result = await _orders.UpdateOneAsync(x => x.Id == e.OrderId, update, cancellationToken: ct);

                    if (result.MatchedCount == 0)
                        _logger.LogWarning("OrderRead not found for status update. OrderId={OrderId}", e.OrderId);

                    break;
                }
			case var t when t == nameof(OrderDeletedIntegrationEvent):
				{
					var e = JsonSerializer.Deserialize<OrderDeletedIntegrationEvent>(payload)!;

					var result = await _orders.DeleteOneAsync(x => x.Id == e.OrderId, ct);

					if (result.DeletedCount == 0)
						_logger.LogWarning("OrderRead not found for delete. OrderId={OrderId}", e.OrderId);

					break;
				}
			case var t when t == nameof(OrderCancelledIntegrationEvent):
				{
					var e = JsonSerializer.Deserialize<OrderCancelledIntegrationEvent>(payload)!;

					var update = Builders<OrderRead>.Update.Set(x => x.Status, "Cancelled");
					var result = await _orders.UpdateOneAsync(x => x.Id == e.OrderId, update, cancellationToken: ct);

					if (result.MatchedCount == 0)
						_logger.LogWarning("OrderRead not found for cancel. OrderId={OrderId}", e.OrderId);

					break;
				}
			default:
                throw new InvalidOperationException($"Unknown outbox message type: {type}");
        }
    }
}