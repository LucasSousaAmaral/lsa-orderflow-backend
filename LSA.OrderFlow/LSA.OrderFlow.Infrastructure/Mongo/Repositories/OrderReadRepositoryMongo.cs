using LSA.OrderFlow.Application.Contracts.Repositories;
using LSA.OrderFlow.Application.Orders.ViewModels;
using LSA.OrderFlow.Infrastructure.Mongo.ReadModels;
using MongoDB.Driver;

namespace LSA.OrderFlow.Infrastructure.Mongo.Repositories
{
    public class OrderReadRepositoryMongo : IOrderReadRepository
    {
        private readonly IMongoCollection<OrderRead> _collection;


        public OrderReadRepositoryMongo(IMongoDatabase db)
        { _collection = db.GetCollection<OrderRead>("orders_read"); }


        public async Task<OrderDetailsVm?> GetDetailsAsync(Guid orderId, CancellationToken ct)
        {
            var doc = await _collection.Find(x => x.Id == orderId).FirstOrDefaultAsync(ct);
            if (doc is null) return null;
            return new OrderDetailsVm(
            doc.Id, doc.CustomerId, doc.OrderDate, doc.Status, doc.TotalAmount,
            doc.Items.Select(i => new OrderItemVm(i.Id, i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.TotalPrice)).ToList()
            );
        }


        public async Task<PagedList<OrderListItemVm>> ListAsync(int page, int pageSize, string? search, CancellationToken ct)
        {
			var notCancelled = Builders<OrderRead>.Filter.Ne(x => x.Status, "Cancelled");

			var searchFilter = string.IsNullOrWhiteSpace(search)
				? Builders<OrderRead>.Filter.Empty
				: Builders<OrderRead>.Filter.Or(
					Builders<OrderRead>.Filter.Regex(x => x.Status, new(search, "i")),
					Builders<OrderRead>.Filter.Regex(x => x.CustomerId.ToString(), new(search, "i"))
				);

			var filter = Builders<OrderRead>.Filter.And(notCancelled, searchFilter);

			var total = await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
            var items = await _collection.Find(filter)
            .SortByDescending(x => x.OrderDate)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .Project(x => new OrderListItemVm(x.Id, x.CustomerId, x.OrderDate, x.Status, x.TotalAmount))
            .ToListAsync(ct);


            return new PagedList<OrderListItemVm>(items, page, pageSize, total);
        }
    }
}