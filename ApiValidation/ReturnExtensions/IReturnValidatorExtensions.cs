using ApiValidations.Descriptors;
using ApiValidations.Scopes;
using PopValidations.Execution;
using PopValidations.Scopes.ForEachs;
using PopValidations.ValidatorInternals;

namespace PopValidations;

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


        //if (fieldDescriptor is IParamDescriptor_Internal<TValidationType, TParamType> converted)
        //{
        //    var forEachScope = new ParamForEachScope<TValidationType, TParamType>(
        //        converted,
        //        actions
        //    );


        return returnDescriptor;
        //}
        //else
        //{
        //    throw new PopValidationException(nameof(fieldDescriptor), "Invalid Field Descriptor");
        //}
    }
}
