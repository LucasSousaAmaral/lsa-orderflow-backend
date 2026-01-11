using FluentValidation;
using LSA.OrderFlow.Domain.Orders;

namespace LSA.OrderFlow.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
	public UpdateOrderCommandValidator()
	{
		RuleFor(x => x.OrderId).NotEmpty();
		RuleFor(x => x.NewStatus).InclusiveBetween((int)OrderStatus.Pending, (int)OrderStatus.Cancelled)
			.When(x => x.NewStatus.HasValue);
		When(x => x.ReplaceItems is not null, () =>
		{
			RuleForEach(x => x.ReplaceItems!).ChildRules(i =>
			{
				i.RuleFor(p => p.ProductId).NotEmpty();
				i.RuleFor(p => p.Quantity).GreaterThan(0);
			});
		});
	}
}