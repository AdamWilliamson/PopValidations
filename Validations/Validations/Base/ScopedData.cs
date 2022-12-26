using System;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Validations.Base;

public interface IScopeData
{
    void ReHome(IFieldDescriptorOutline fieldDescriptorOutline);
    Task Init(object? instance);
    object? GetValue();
    string Describe();
    
    void SetParent(IScopeData parent);
}

public interface IScopeData<TResponse> : IScopeData { }

public class ScopedData<TPassThrough, TResponse> : IScopeData
{
    private Func<TPassThrough, Task<TResponse>> PassThroughFunction { get; }
    protected IScopeData? Parent { get; set; } = null;
    TResponse? RetrievedValue = default(TResponse);

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
        if (Parent != null)
        {
            await Parent.Init(instance);
            var parentValue = Parent.GetValue();

            if (parentValue is not TPassThrough)
            {
                throw new Exception();
            }

            RetrievedValue = await PassThroughFunction.Invoke((TPassThrough)parentValue);
        }
        else
        {
            if (instance is not TPassThrough)
            {
                throw new Exception();
            }

            RetrievedValue = await PassThroughFunction.Invoke((TPassThrough)instance);
        }
    }

    public object? GetValue()
    {
        return RetrievedValue;
    }

    public string Describe()
    {
        return "No";
    }

    public ScopedData<TPassThrough, TNewResponse> To<TNewResponse>(
        Func<TPassThrough, Task<TNewResponse>> passThroughFunction
    )
    {
        return new ScopedData<TPassThrough, TNewResponse>(this, passThroughFunction);
    }
}
