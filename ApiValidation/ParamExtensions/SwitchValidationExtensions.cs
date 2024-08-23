using ApiValidations.Descriptors;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System.Diagnostics;

namespace ApiValidations;

public class ParamSwitch<TParamType, TValidationType, TScopedDataType>
{
    private ParamDescriptor<TParamType, TValidationType> parent;
    private readonly IScopedData<TScopedDataType> scopedData;

    public ParamSwitch(
        ParamDescriptor<TParamType, TValidationType> parent,
        IScopedData<TScopedDataType> scopedData
    )
    {
        this.parent = parent;
        this.scopedData = scopedData;
    }

    public ParamSwitch<TParamType, TValidationType, TScopedDataType> Case(
        string description, 
        Func<TScopedDataType?, bool> Test,
        string ErrorMessage)
    {
        var validation = new IsCustomValidation<TParamType>(
            description,
            ErrorMessage,
            new ScopedData<TParamType, bool>("",
                (x) => {
                    Debug.Assert(x is TParamType || x is TParamType? || x is null);
                    Debug.Assert(scopedData?.GetValue() is TScopedDataType || scopedData?.GetValue() is null);
                    var data = scopedData?.GetValue();

                    if (data is TScopedDataType)
                        return !(Test?.Invoke((TScopedDataType)data) ?? true);
                    else if (data is null)
                        return !(Test?.Invoke(default(TScopedDataType)) ?? true);
                    else
                        throw new ScopedDataException("Switch");
                }
            )
        );

        var result = parent.AddValidation(validation);
        parent = result;
        return this;
    }

    public ParamSwitch<TParamType, TValidationType, TScopedDataType> Ignore(
        string description,
        Func<TScopedDataType?, bool> Test)
    {
        return this;
    }

    public ParamDescriptor<TParamType, TValidationType> End()
    {
        return parent;
    }
}

public static partial class SwitchValidationExtensions
{
    public static ParamSwitch<TParamType, TValidationType, TScopedDataType> Switch<TParamType, TValidationType, TScopedDataType>(
        this ParamDescriptor<TParamType, TValidationType> fieldDescriptor,
        string scopedDataDescription,
        Func<TValidationType, Task<TScopedDataType>> dataRetrievalFunc
    )
    {
        return new ParamSwitch<TParamType, TValidationType, TScopedDataType>( fieldDescriptor,
            new ScopedData<TValidationType, TScopedDataType>(scopedDataDescription, dataRetrievalFunc)
        );
    }
}
