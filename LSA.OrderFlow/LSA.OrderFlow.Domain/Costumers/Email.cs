using LSA.OrderFlow.Domain.Abstraction;
using LSA.OrderFlow.Domain.Shared;
using System.Text.RegularExpressions;

namespace LSA.OrderFlow.Domain.Costumers;

public sealed class Email : ValueObject
{
    private static readonly Regex Pattern =
        new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        Guard.AgainstNull(value, nameof(value));
        value = value.Trim().ToLowerInvariant();
        Guard.Against(nameof(Email), !Pattern.IsMatch(value), "invalid email format");
        return new Email(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}