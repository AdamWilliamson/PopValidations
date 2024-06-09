using ApiValidations.Descriptors;
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

    //public static ParamDescriptor<IEnumerable<TParamType>, TValidationType> ForEach<
    //    TParamType, TValidationType
    //>(
    //    this ParamDescriptor<IEnumerable<TParamType>, TValidationType> fieldDescriptor,
    //    Action<ParamDescriptor<IEnumerable<TParamType>, TValidationType>> actions
    //)
    //    where TValidationType : class
    //{

    //    //if (fieldDescriptor is IFieldDescripor_Internal<TValidationType, IEnumerable<TParamType>> converted)
    //    //{
    //        var forEachScope = new ForEachScope<TValidationType, TParamType>(
    //            converted,
    //            actions
    //        );

    //        fieldDescriptor.AddSelfDescribingEntity(forEachScope);
    //        return fieldDescriptor;
    //    //}
    //    //else
    //    //{
    //    //    throw new PopValidationException(nameof(fieldDescriptor), "Invalid Field Descriptor");
    //    //}
    //}
}
