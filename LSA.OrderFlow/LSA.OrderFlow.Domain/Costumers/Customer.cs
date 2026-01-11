using LSA.OrderFlow.Domain.Abstraction;
using LSA.OrderFlow.Domain.Costumers;
using LSA.OrderFlow.Domain.Shared;

namespace LSA.OrderFlow.Domain.Customers;

public sealed class Customer : Entity
{
	public Guid Id { get; private set; }
	public string Name { get; private set; } = default!;
	public Email Email { get; private set; } = default!;
	public string? Phone { get; private set; }

	//Para o EF
	private Customer() { }

	private Customer(Guid id, string name, Email email, string? phone)
	{
		Id = id;
		Name = name;
		Email = email;
		Phone = phone;
	}

	public static Customer Create(string name, Email email, string? phone = null)
	{
		Guard.Against(nameof(name), string.IsNullOrWhiteSpace(name), "cannot be empty");
		return new Customer(Guid.NewGuid(), name.Trim(), email, phone?.Trim());
	}
	public static Customer Create(Guid id, string name, Email email, string? phone = null)
	{
		Guard.Against(nameof(id), id == Guid.Empty, "invalid customer id");
		Guard.Against(nameof(name), string.IsNullOrWhiteSpace(name), "cannot be empty");
		return new Customer(id, name.Trim(), email, phone?.Trim());
	}

	public void Rename(string name)
	{
		Guard.Against(nameof(name), string.IsNullOrWhiteSpace(name), "cannot be empty");
		Name = name.Trim();
	}

	public void ChangeEmail(Email email) => Email = email;
}
