using System;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Validations.Base;

public interface IScopedData<TResponse> : IScopeData 
{
    ScopedData<TResponse, TNewResponse> To<TNewResponse>(string newDescription,
        Func<TResponse, Task<TNewResponse>> passThroughFunction
    );
}

public class ScopedData<TResponse> : IScopedData<TResponse>, IScopeData
{
    private Func<Task<TResponse>>? PassThroughFunction { get; }
    
    private TResponse? RetrievedValue = default(TResponse);
    private bool HasRetrievedValue = false;
    private string? Description = null;

    public ScopedData(TResponse data)
    {
        RetrievedValue = data;
        HasRetrievedValue = true;
    }

    public ScopedData(string? description, Func<Task<TResponse>> passThroughFunction)
    {
        Description = description;
        PassThroughFunction = passThroughFunction;
    }

    public void SetParent(IScopeData parent)
    {
        // Unnecessary
    }

    public void ReHome(IFieldDescriptorOutline fieldDescriptorOutline)
    {
        // Unnecessary
    }

    public async Task Init(object? instance)
    {
        if (HasRetrievedValue) return;

        if (PassThroughFunction == null)
            throw new Exception("Passthrough should NOT be null");

        RetrievedValue = await PassThroughFunction.Invoke();
        HasRetrievedValue = true;
    }

    public object? GetValue()
    {
        if (HasRetrievedValue)
            return RetrievedValue;

        Init(null).Wait();
        return RetrievedValue;
    }

    public string Describe()
    {
        if (Description != null) return Description;

        if (HasRetrievedValue) return RetrievedValue?.ToString() ?? String.Empty;

        return String.Empty;
    }

    public ScopedData<TResponse, TNewResponse> To<TNewResponse>(
        string newDescription,
        Func<TResponse, Task<TNewResponse>> passThroughFunction
    )
    {
        return new ScopedData<TResponse, TNewResponse>(this, newDescription, passThroughFunction);
    }
}


public class ScopedData<TPassThrough, TResponse> : IScopedData<TResponse>, IScopeData
{
    private Func<TPassThrough, Task<TResponse>>? PassThroughFunction { get; }
    protected IScopeData? Parent { get; set; } = null;
    private TResponse? RetrievedValue = default(TResponse);
    private bool HasRetrievedValue = false;
    private string? Description = null;

    public ScopedData(TResponse data)
    {
        RetrievedValue = data;
        HasRetrievedValue = true;
    }

    public ScopedData(string? description, Func<TPassThrough, Task<TResponse>> passThroughFunction)
    {
        Description = description;
        PassThroughFunction = passThroughFunction;
    }

    public ScopedData(IScopeData parent, string newDescription, Func<TPassThrough, Task<TResponse>> passThroughFunction)
    {
        Parent = parent;
        Description = newDescription;
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
                Description,
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
        if (HasRetrievedValue)
            return RetrievedValue;

        if (Parent != null)
            Init(Parent.GetValue()).Wait();

        return RetrievedValue;
    }

    public string Describe()
    {
        if (Description != null) return Description;

        if (HasRetrievedValue) return RetrievedValue?.ToString() ?? String.Empty;

        return String.Empty;
    }

    public ScopedData<TResponse, TNewResponse> To<TNewResponse>(
        string newDescription,
        Func<TResponse, Task<TNewResponse>> passThroughFunction
    )
    {
        return new ScopedData<TResponse, TNewResponse>(this, newDescription, passThroughFunction);
    }
}
