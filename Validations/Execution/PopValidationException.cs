using System;

namespace PopValidations.Execution;

public class PopValidationException : Exception
{
    public string Property { get; private set; }

    public PopValidationException(string property, string message)
        : base(message)
    {
        Property = property;
    }
}
