using ApiValidations.Descriptors;
using ApiValidations.Descriptors.Core;
using ApiValidations.Scopes;
using PopValidations.Execution;
using PopValidations.ValidatorInternals;

namespace ApiValidations;

public static partial class IReturnValidatorExtensions
{
    public static IReturnDescriptor<TReturnType> Vitally<TReturnType>(this IReturnDescriptor<TReturnType> fieldDescriptor)
    {
        fieldDescriptor.NextValidationIsVital();
        return fieldDescriptor;
    }

    public static IReturnDescriptor<TReturnType> AllNextAreVital<TReturnType>(this IReturnDescriptor<TReturnType> fieldDescriptor)
    {
        fieldDescriptor.SetAlwaysVital();
        return fieldDescriptor;
    }

    public static IReturnDescriptor<TReturnType> SetValidator<TReturnType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        ISubValidatorClass<TReturnType> validatorClass
    )
    {
        fieldDescriptor.AddSubValidator(validatorClass);
        return fieldDescriptor;
    }

    public static IReturnDescriptor<IEnumerable<TParamType>> ForEach<
           TParamType
       >(
           this IReturnDescriptor<IEnumerable<TParamType>> returnDescriptor,
           Action<IReturnDescriptor<TParamType>> actions
       )
    {
        if (returnDescriptor is IReturnDescriptor_Internal converted)
        {
            var forEachScope = new ForEachReturnScope<TParamType>(
                converted.FunctionDescriptor,
                returnDescriptor,
                actions
            );

            returnDescriptor.AddSelfDescribingEntity(forEachScope);
        }
        else
        {
            throw new PopValidationException(nameof(returnDescriptor), "Invalid Param Descriptor");
        }

        return returnDescriptor;
    }
}
