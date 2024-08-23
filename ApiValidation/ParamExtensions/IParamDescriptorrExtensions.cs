using ApiValidations.Descriptors;
using PopValidations.Execution;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes.ForEachs;
using PopValidations.ValidatorInternals;

namespace ApiValidations;

public interface IParamDescripor_Internal<TValidationType, TFieldType> 
    : IParamDescriptor<TFieldType>
{
    IPropertyExpressionToken<TFieldType> PropertyToken { get; }
    IValidationStore Store { get; }
}

public static partial class IParamDescriptorrExtensions
{
    public static ParamDescriptor<TParamType, TValidationType> Vitally<
        TParamType, TValidationType
    >(this ParamDescriptor<TParamType, TValidationType> paramDescriptor)
    {
        return paramDescriptor.NextValidationIsVital();
    }

    public static ParamDescriptor<TParamType, TValidationType> AllNextAreVital<
        TParamType, TValidationType
    >(this ParamDescriptor<TParamType, TValidationType> paramDescriptor)
    {
        return paramDescriptor.SetAlwaysVital();
    }

    public static ParamDescriptor<TParamType, TValidationType> SetValidator<
        TParamType, TValidationType
    >(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor,
        ISubValidatorClass<TParamType> validatorClass
    )
    {
        return paramDescriptor.AddSubValidator(validatorClass);
    }

    public static ParamDescriptor<IEnumerable<TParamType>, TValidationType> ForEach<
        TParamType, TValidationType
    >(
        this ParamDescriptor<IEnumerable<TParamType>, TValidationType> paramDescriptor,
        Func<ParamDescriptor<TParamType, TValidationType>, ParamDescriptor<TParamType, TValidationType>> actions
    )
        where TValidationType : class
    {
        if (paramDescriptor is IParamDescriptor_Internal<IEnumerable<TParamType>> converted)
        {
            var forEachScope = new ParamForEachScope<TValidationType, IEnumerable<TParamType>, TParamType>(
                converted.ParamVisitor,
                paramDescriptor,
                actions
            );

            return paramDescriptor.AddSelfDescribingEntity(forEachScope);
        }
        else
        {
            throw new PopValidationException(nameof(paramDescriptor), "Invalid Param Descriptor");
        }


        //if (fieldDescriptor is IParamDescriptor_Internal<TValidationType, TParamType> converted)
        //{
        //    var forEachScope = new ParamForEachScope<TValidationType, TParamType>(
        //        converted,
        //        actions
        //    );

        
        //return paramDescriptor;
        //}
        //else
        //{
        //    throw new PopValidationException(nameof(fieldDescriptor), "Invalid Field Descriptor");
        //}
    }
}
