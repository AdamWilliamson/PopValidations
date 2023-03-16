namespace PopValidations.MediatR
{
    public class ValidationException : Exception
    {
        public ValidationException(Dictionary<string, List<string>> errors)
        {
            Errors = errors;
        }

        public Dictionary<string, List<string>> Errors { get; }
    }
}