using FluentValidation;
using MediatR;

namespace LSA.OrderFlow.Application.Common.Validators;

public sealed class FluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public FluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (_validators.Any())
        {
            var ctx = new ValidationContext<TRequest>(request);
            var failures = (await Task.WhenAll(_validators.Select(v => v.ValidateAsync(ctx, ct))))
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();
            if (failures.Count != 0)
                throw new ValidationException(failures);
        }
        return await next();
    }
}