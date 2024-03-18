using PopValidations.Execution.Validations.Base;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;
using PopValidations_Functional_Testbed;

namespace ApiValidations;

public static class Extensions
{
    public static ParamDescriptor<TParamType, TValidationType> IsNotNull<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor
    )
    {
        return paramDescriptor.AddValidation(new IsNotNullValidation());
    }

    public static ParamDescriptor<TParamType, TValidationType> IsNotNull<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor,
        Action<ValidationOptions>? optionsAction
    )
    {
        var validation = new IsNotNullValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        return paramDescriptor.AddValidation(validation);
    }


    public static ParamDescriptor<TParamType, TValidationType> IsEmail<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor
)
    {
        return paramDescriptor.AddValidation(new IsEmailValidation());
    }
    public static ParamDescriptor<TParamType, TValidationType> IsEmail<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var validation = new IsEmailValidation();
        optionsAction?.Invoke(new ValidationOptions(validation));
        return paramDescriptor.AddValidation(validation);
    }







    public static ParamDescriptor<TParamType, TValidationType> IsGreaterThan<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor,
        IComparable value
    )
    {
        var scopedData = new ScopedData<TValidationType, IComparable>(value);
        var validation = new IsGreaterThanValidation(scopedData);
        return paramDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsGreaterThan<TParamType, TValidationType, TPassThrough>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor,
        IScopedData<TPassThrough?> scopedData
    )
        where TPassThrough : IComparable
    {
        return paramDescriptor.AddValidation(new IsGreaterThanValidation(scopedData));
    }



    public static ParamDescriptor<TParamType, TValidationType> IsGreaterThan<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor,
        IComparable value,
        Action<ValidationOptions>? optionsAction = null
    )
    {
        var scopedData = new ScopedData<TValidationType, IComparable>(value);
        var validation = new IsGreaterThanValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        return paramDescriptor.AddValidation(validation);
    }

    public static ParamDescriptor<TParamType, TValidationType> IsGreaterThan<TParamType, TValidationType, TPassThrough>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor,
        IScopedData<TPassThrough?> scopedData,
        Action<ValidationOptions>? optionsAction = null
    )
        where TPassThrough : IComparable
    {
        var validation = new IsGreaterThanValidation(scopedData);
        optionsAction?.Invoke(new ValidationOptions(validation));
        return paramDescriptor.AddValidation(validation);
    }





    public static ParamDescriptor<TParamType, TValidationType> SetValidator<TParamType, TValidationType>(
        this ParamDescriptor<TParamType, TValidationType> paramDescriptor,
        ISubValidatorClass<TParamType> validatorClass
    )
    {
        return paramDescriptor.AddSubValidator(validatorClass);
    }

    //===========================================================================
    public static IReturnDescriptor<TReturnType> IsNotNull<TReturnType>(
        this IReturnDescriptor<TReturnType> returnDescriptor
    )
    {
        var validation = new IsNotNullValidation();
        returnDescriptor.AddValidation(validation);
        return returnDescriptor;
    }

    public static IReturnDescriptor IsNotNull(
        this IReturnDescriptor returnDescriptor
    )
    {
        var validation = new IsNotNullValidation();
        returnDescriptor.AddValidation(validation);
        return returnDescriptor;
    }
}

