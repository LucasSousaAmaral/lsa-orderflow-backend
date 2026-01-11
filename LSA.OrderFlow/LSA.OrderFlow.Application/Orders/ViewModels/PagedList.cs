namespace LSA.OrderFlow.Application.Orders.ViewModels;

public record PagedList<T>(IReadOnlyList<T> Items, int Page, int PageSize, long TotalCount);