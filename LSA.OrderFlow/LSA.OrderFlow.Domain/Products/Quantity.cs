using LSA.OrderFlow.Domain.Abstraction;
using LSA.OrderFlow.Domain.Shared;

namespace LSA.OrderFlow.Domain.Products;

public sealed class Quantity : ValueObject
{
    public int Value { get; }

    private Quantity(int value) => Value = value;

    public static Quantity Create(int value)
    {
        Guard.Against(nameof(Quantity), value <= 0, "must be greater than zero");
        return new Quantity(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator int(Quantity q) => q.Value;
}
