using LSA.OrderFlow.Domain.Abstraction;
using LSA.OrderFlow.Domain.Shared;
namespace LSA.OrderFlow.Domain.Products;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; } = "BRL";

    private Money(decimal amount, string currency)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
        Currency = currency;
    }

    public static Money Zero => new(0m, "BRL");

    public static Money Create(decimal amount, string currency = "BRL")
    {
        Guard.Against(nameof(Money), amount < 0m, "amount cannot be negative");
        return new Money(amount, currency);
    }

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency) throw new InvalidOperationException("currency mismatch");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator *(Money a, int qty) => new(a.Amount * qty, a.Currency);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency} {Amount:0.00}";
}
