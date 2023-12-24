namespace PopValidations.MediatR;

public class PopValidationMediatRException : Exception
{
    public PopValidationMediatRException(Dictionary<string, List<string>> errors)
    {
        Errors = errors;
    }

    public Dictionary<string, List<string>> Errors { get; }
}
