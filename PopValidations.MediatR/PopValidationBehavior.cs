using MediatR;
using PopValidations.Execution;
using PopValidations.ValidatorInternals;

namespace PopValidations.MediatR;

public class PopValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IMainValidator<TRequest>> _validators;
    private readonly IValidationRunner<TRequest> runner;

    public PopValidationBehavior(
        IEnumerable<IMainValidator<TRequest>> validators,
        IValidationRunner<TRequest> runner
    )
    {
        _validators = validators;
        this.runner = runner;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken
    )
    {
        //var errors = new List<ValidationResult>();
        Dictionary<string, List<string>> errors = new ();
        foreach(var validator in _validators)
        {
            var errorResult = await runner.Validate(request);
            foreach(var error in errorResult.Errors)
            {
                if (!errors.ContainsKey(error.Key))
                {
                    errors.Add(error.Key, new());
                }

                errors[error.Key].AddRange(error.Value.Where(x => !errors[error.Key].Contains(x)));
            }
        }

        //var failures = _validators
        //    .Select(v => v.Validate(request))
        //    .SelectMany(result => result.Errors)
        //    .Where(f => f != null)
        //    .ToList();

        if (errors.Any())
        {
            throw new PopValidationException(errors);
        }

        return await next();
    }
}
