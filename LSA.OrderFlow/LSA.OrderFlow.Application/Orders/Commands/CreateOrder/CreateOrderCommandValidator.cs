using FluentValidation;

namespace LSA.OrderFlow.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
	public CreateOrderCommandValidator()
	{
		RuleFor(x => x.CustomerId).NotEmpty();
		RuleFor(x => x.Items).NotEmpty();
		RuleForEach(x => x.Items).ChildRules(i =>
		{
			i.RuleFor(p => p.ProductId).NotEmpty();
			i.RuleFor(p => p.Quantity).GreaterThan(0);
		});
	}
}