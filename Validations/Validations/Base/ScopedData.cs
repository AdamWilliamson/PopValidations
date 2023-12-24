using System;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Validations.Base;

public class ScopedDataException : Exception
{
    public ScopedDataException() : base("ScopedData did somethng unexpected") { }
    public ScopedDataException(string message) : base(message) { }
}

public interface IScopedData<TResponse> : IScopeData 
{
    ScopedData<TResponse, TNewResponse> To<TNewResponse>(string newDescription,
        Func<TResponse, Task<TNewResponse>> passThroughFunction
    );

    ScopedData<TResponse, TNewResponse> To<TNewResponse>(string newDescription,
        Func<TResponse, TNewResponse> passThroughFunction
    );

    ScopedData<TInput, TNewResponse> To<TInput, TNewResponse>(
        string newDescription,
        Func<TInput, TResponse?, TNewResponse> passThroughFunction
    );

    TResponse GetTypedValue();
}

public class ScopedData<TResponse> : IScopedData<TResponse>, IScopeData
{
    private Func<Task<TResponse>>? PassThroughFunction { get; }

#pragma warning disable CS8601 // Possible null reference assignment.
    private TResponse RetrievedValue = default;
#pragma warning restore CS8601 // Possible null reference assignment.
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
    }

    public void ReHome(IFieldDescriptorOutline fieldDescriptorOutline)
    {
    }

    public async Task Init(object? instance)
    {
        if (HasRetrievedValue) return;

        if (PassThroughFunction == null)
            throw new ScopedDataException("Passthrough should NOT be null");

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

    public TResponse GetTypedValue()
    {
        try 
        {
            if (HasRetrievedValue)
                return RetrievedValue;

            Init(null).Wait();
            return RetrievedValue;
        }
        catch
        {
            throw;
        }
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

    public ScopedData<TResponse, TNewResponse> To<TNewResponse>(
        string newDescription,
        Func<TResponse, TNewResponse> passThroughFunction
    )
    {
        return new ScopedData<TResponse, TNewResponse>(this, newDescription, passThroughFunction);
    }

    public ScopedData<TInput, TNewResponse> To<TInput, TNewResponse>(
        string newDescription,
        Func<TInput, TResponse?, TNewResponse> passThroughFunction
    )
    {
        var func = (TInput val) => passThroughFunction.Invoke(val, this.GetTypedValue() ?? default);
        return new ScopedData<TInput, TNewResponse>(newDescription, func);
    }
}


public interface IScopedData<TInput, TResponse> : IScopedData<TResponse> { }
public class ScopedData<TPassThrough, TResponse> : IScopedData<TResponse>, IScopedData<TPassThrough, TResponse>, IScopeData
{
    private Func<TPassThrough, Task<TResponse>>? PassThroughFunction { get; }
    protected IScopeData? Parent { get; set; } = null;
#pragma warning disable CS8601 // Possible null reference assignment.
    private TResponse RetrievedValue = default;
#pragma warning restore CS8601 // Possible null reference assignment.
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

    public ScopedData(string? description, Func<TPassThrough, TResponse> passThroughFunction)
    {
        Description = description;
        PassThroughFunction = (TPassThrough v) => Task.FromResult(passThroughFunction.Invoke(v));
    }

    public ScopedData(IScopeData parent, string newDescription, Func<TPassThrough, Task<TResponse>> passThroughFunction)
    {
        Parent = parent;
        Description = newDescription;
        PassThroughFunction = passThroughFunction;
    }

    public ScopedData(IScopeData parent, string newDescription, Func<TPassThrough, TResponse> passThroughFunction)
    {
        Parent = parent;
        Description = newDescription;
        PassThroughFunction = (TPassThrough v) => Task.FromResult(passThroughFunction.Invoke(v));
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
                        throw new ScopedDataException();
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

            if (parentValue is not TPassThrough 
                && !(parentValue == null && typeof(TPassThrough).IsGenericType && typeof(TPassThrough).GetGenericTypeDefinition() == typeof(Nullable<>))
                && typeof(TPassThrough).IsValueType
            )
            {
                throw new ScopedDataException();
            }

            if (PassThroughFunction == null)
                throw new ScopedDataException("Passthrough should NOT be null");

            if (parentValue == null)
#pragma warning disable CS8604 // Possible null reference argument.
                RetrievedValue = await PassThroughFunction.Invoke(default);
#pragma warning restore CS8604 // Possible null reference argument.
            else
                RetrievedValue = await PassThroughFunction.Invoke((TPassThrough)parentValue);
        }
        else
        {
            if (instance is not TPassThrough
                && !(typeof(TPassThrough).IsGenericType && typeof(TPassThrough).GetGenericTypeDefinition() == typeof(Nullable<>))
                && typeof(TPassThrough).IsValueType)
            {
                throw new ScopedDataException("Invalid Data Type.");
            }

            if (PassThroughFunction == null)
                throw new ScopedDataException("Passthrough should NOT be null");

            if (instance == null)
#pragma warning disable CS8604 // Possible null reference argument.
                RetrievedValue = await PassThroughFunction.Invoke(default);
#pragma warning restore CS8604 // Possible null reference argument.
            else
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

    public TResponse GetTypedValue()
    {
        try
        {
            if (HasRetrievedValue)
                return RetrievedValue;

            if (Parent != null)
                Init(Parent.GetValue()).Wait();

            return RetrievedValue;
        }
        catch
        {
            throw;
        }
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

    public ScopedData<TResponse, TNewResponse> To<TNewResponse>(
        string newDescription,
        Func<TResponse, TNewResponse> passThroughFunction
    )
    {
        return new ScopedData<TResponse, TNewResponse>(this, newDescription, passThroughFunction);
    }

    public ScopedData<TInput, TNewResponse> To<TInput, TNewResponse>(
         string newDescription,
         Func<TInput, TResponse, TNewResponse> passThroughFunction
     )
    {
        var func = (TInput val) => passThroughFunction.Invoke(val, this.GetTypedValue());
        return new ScopedData<TInput, TNewResponse>(newDescription, func);
    }
}
