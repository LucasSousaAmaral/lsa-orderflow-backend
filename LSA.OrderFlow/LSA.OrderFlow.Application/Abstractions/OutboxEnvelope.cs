namespace LSA.OrderFlow.Application.Abstractions;

public sealed record OutboxEnvelope(string Type, string Payload, DateTime OccurredOnUtc);