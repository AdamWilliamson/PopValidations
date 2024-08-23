using ApiValidations.Descriptors;
using ApiValidations.Descriptors.Core;
using PopValidations.Validations;
using PopValidations.Validations.Base;
using System.Diagnostics;

namespace ApiValidations;

public class ReturnSwitch<TReturnType, TScopedDataType>
{
    private IReturnDescriptor<TReturnType> parent;
    private readonly IScopedData<TScopedDataType> scopedData;
    private IFunctionContext context;

    public ReturnSwitch(
        IReturnDescriptor<TReturnType> parent,
        IScopedData<TScopedDataType> scopedData,
        IFunctionContext context
    )
    {
        this.parent = parent;
        this.scopedData = scopedData;
        this.context = context;
    }

    public ReturnSwitch<TReturnType, TScopedDataType> Case(
        string description, 
        Func<TScopedDataType?, bool> Test,
        string ErrorMessage)
    {
        var validation = new IsCustomValidation<TReturnType>(
            description,
            ErrorMessage,
            new ScopedData<TReturnType, bool>("",
                (x) => {
                    Debug.Assert(x is TReturnType || x is TReturnType? || x is null);
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

        parent.AddValidation(validation);
        return this;
    }

    public ReturnSwitch<TReturnType, TScopedDataType> Case(
       string description,
       Func<TScopedDataType?, IFunctionContext, bool> Test,
       string ErrorMessage)
    {
        var validation = new IsCustomValidation<TReturnType>(
            description,
            ErrorMessage,
            new ScopedData<TReturnType, bool>("",
                (x) => {
                    Debug.Assert(x is TReturnType || x is TReturnType? || x is null);
                    Debug.Assert(scopedData?.GetValue() is TScopedDataType || scopedData?.GetValue() is null);
                    var data = scopedData?.GetValue();

                    if (data is TScopedDataType)
                        return !(Test?.Invoke((TScopedDataType)data, context) ?? true);
                    else if (data is null)
                        return !(Test?.Invoke(default(TScopedDataType), context) ?? true);
                    else
                        throw new ScopedDataException("Switch");
                }
            )
        );

        parent.AddValidation(validation);
        return this;
    }

    public ReturnSwitch<TReturnType, TScopedDataType> Ignore(
        string description,
        Func<TScopedDataType?, bool> Test)
    {
        return this;
    }

    public IReturnDescriptor<TReturnType> End()
    {
        return parent;
    }
}

public static partial class SwitchValidationExtensions
{
    public static ReturnSwitch<TReturnType, TScopedDataType> Switch<TReturnType, TScopedDataType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        string scopedDataDescription,
        Func<Task<TScopedDataType>> dataRetrievalFunc
    )
    {
        if (fieldDescriptor is IReturnDescriptor_Internal internalDescriptor)
        {
            return new ReturnSwitch<TReturnType, TScopedDataType>(
                fieldDescriptor,
                new ScopedData<TScopedDataType>(scopedDataDescription, dataRetrievalFunc),
                internalDescriptor.GetContext()
            );
        }

        throw new Exception("Invalid Return Descriptor");
    }

    public static ReturnSwitch<TReturnType, TScopedDataType> Switch<TReturnType, TScopedDataType>(
        this IReturnDescriptor<TReturnType> fieldDescriptor,
        string scopedDataDescription,
        Func<IFunctionContext, Task<TScopedDataType>> dataRetrievalFunc
    )
    {
        if (fieldDescriptor is IReturnDescriptor_Internal internalDescriptor)
        {
            return new ReturnSwitch<TReturnType, TScopedDataType>(
                fieldDescriptor,
                new ScopedData<IFunctionContext, TScopedDataType>(scopedDataDescription, dataRetrievalFunc),
                internalDescriptor.GetContext()
            );
        }

        throw new Exception("Invalid Return Descriptor");
    }
}
