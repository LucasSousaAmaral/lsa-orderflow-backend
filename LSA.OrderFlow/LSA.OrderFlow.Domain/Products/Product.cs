using LSA.OrderFlow.Domain.Abstraction;
using LSA.OrderFlow.Domain.Shared;

namespace LSA.OrderFlow.Domain.Products;

public sealed class Product : Entity
{
	public Guid Id { get; }
	public string Name { get; private set; }
	public Money UnitPrice { get; private set; }

	//Para o EF
	private Product() { }

	private Product(Guid id, string name, Money unitPrice)
	{
		Id = id;
		Name = name;
		UnitPrice = unitPrice;
	}

	public static Product Create(string name, Money unitPrice)
	{
		Guard.Against(nameof(name), string.IsNullOrWhiteSpace(name), "cannot be empty");
		return new Product(Guid.NewGuid(), name.Trim(), unitPrice);
	}

	public static Product Create(Guid id, string name, Money unitPrice)
	{
		Guard.Against(nameof(id), id == Guid.Empty, "invalid product id");
		Guard.Against(nameof(name), string.IsNullOrWhiteSpace(name), "cannot be empty");
		return new Product(id, name.Trim(), unitPrice);
	}

	public void Rename(string name)
	{
		Guard.Against(nameof(name), string.IsNullOrWhiteSpace(name), "cannot be empty");
		Name = name.Trim();
	}

	public void Reprice(Money price) => UnitPrice = price;
}