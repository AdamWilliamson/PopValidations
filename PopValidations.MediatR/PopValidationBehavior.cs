using MediatR;
using PopValidations.Execution;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;

namespace PopValidations.MediatR;

public class PopValidationBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
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
        Dictionary<string, List<string>> errors = new ();
        foreach(var validator in _validators)
        {
            try
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
            catch (ValidationException exception)
            {
                if (!errors.ContainsKey("ValidationException"))
                    errors.Add("ValidationException", new());

                errors["ValidationException"].Add(exception.Message);
            }
            catch(PopValidationException exception)
            {
                if (!errors.ContainsKey(exception.Property))
                    errors.Add(exception.Property, new());

                errors[exception.Property].Add(exception.Message);
            }
            catch(Exception exception)
            {
                if (!errors.ContainsKey("PopValidationException"))
                    errors.Add("PopValidationException", new());

                errors["PopValidationException"].Add(exception.Message);
            }
        }

        if (errors.Any())
        {
            throw new PopValidationHttpException(errors);
        }

        return await next();
    }
}
