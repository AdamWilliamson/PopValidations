using System;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Validations.Base;

public class ScopedData<TPassThrough, TResponse> : IScopeData
{
    private Func<TPassThrough, Task<TResponse>>? PassThroughFunction { get; }
    protected IScopeData? Parent { get; set; } = null;
    private TResponse? RetrievedValue = default(TResponse);
    private bool HasRetrievedValue = false;

    public ScopedData(TResponse data)
    {
        RetrievedValue = data;
        HasRetrievedValue = true;
    }

    public ScopedData(Func<TPassThrough, Task<TResponse>> passThroughFunction)
    {
        PassThroughFunction = passThroughFunction;
    }

    public ScopedData(IScopeData parent, Func<TPassThrough, Task<TResponse>> passThroughFunction)
    {
        Parent = parent;
        PassThroughFunction = passThroughFunction;
    }

    public void SetParent(IScopeData parent)
    {
        if (Parent == null)
        {
            Parent = parent;
        }
        else
        {
            Parent.SetParent(parent);
        }
    }

    public void ReHome(IFieldDescriptorOutline fieldDescriptorOutline)
    {
        if (fieldDescriptorOutline == null) return;

        SetParent(
            new ScopedData<object?, TPassThrough>(
                (instance) => {
                    var result = fieldDescriptorOutline.GetValue(instance);
                    if (result is not TPassThrough)
                    {
                        throw new Exception();
                    }
                    return Task.FromResult((TPassThrough)result);
                }
            )
        );
    }

    public async Task Init(object? instance)
    {
        if (HasRetrievedValue) return;

        if (Parent != null)
        {
            await Parent.Init(instance);
            var parentValue = Parent.GetValue();

            if (parentValue is not TPassThrough)
            {
                throw new Exception();
            }

            if (PassThroughFunction == null)
                throw new Exception("Passthrough should NOT be null");

            RetrievedValue = await PassThroughFunction.Invoke((TPassThrough)parentValue);
        }
        else
        {
            if (instance is not TPassThrough)
            {
                throw new Exception();
            }

            if (PassThroughFunction == null)
                throw new Exception("Passthrough should NOT be null");

            RetrievedValue = await PassThroughFunction.Invoke((TPassThrough)instance);
        }

        HasRetrievedValue = true;
    }

    public object? GetValue()
    {
        return RetrievedValue;
    }

    public string Describe()
    {
        if (HasRetrievedValue) return RetrievedValue?.ToString() ?? String.Empty;

        return String.Empty;
    }

    public ScopedData<TPassThrough, TNewResponse> To<TNewResponse>(
        Func<TPassThrough, Task<TNewResponse>> passThroughFunction
    )
    {
        return new ScopedData<TPassThrough, TNewResponse>(this, passThroughFunction);
    }
}
