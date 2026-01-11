namespace LSA.OrderFlow.Domain.Shared;

public static class Guard
{
    public static void AgainstNull(object? value, string name)
    {
        if (value is null) throw new ArgumentNullException(name);
    }
    public static void Against(string name, bool condition, string message)
    {
        if (condition) throw new DomainException($"{name}: {message}");
    }
}
