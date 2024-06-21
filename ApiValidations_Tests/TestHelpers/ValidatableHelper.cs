using System.Reflection;

namespace ApiValidations_Tests.TestHelpers;

[Flags]
public enum ValidatableType
{
    Parameters,
    Exceptions,
    Return,
    NoExceptions,
    All = Parameters & Exceptions & Return
}

public static class ValidatableHelper
{
    public static int GetValidatableCount<T>(ValidatableType validations = ValidatableType.All)
    {
        int count = 0;
        var methods = typeof(T).GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        foreach (var method in methods)
        {
            if (validations.HasFlag(ValidatableType.Parameters))
            {
                count += method.GetParameters().Length;
            }

            if (
                validations.HasFlag(ValidatableType.NoExceptions) && method.ReturnType != typeof(void)
                || !validations.HasFlag(ValidatableType.NoExceptions)
            )
            {
                count += 1;
            }
        }

        return count;
    }
}
