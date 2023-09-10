﻿using System;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Validations.Base;

public interface IScopedData<TResponse> : IScopeData 
{
    ScopedData<TResponse, TNewResponse> To<TNewResponse>(string newDescription,
        Func<TResponse, Task<TNewResponse>> passThroughFunction
    );

    ScopedData<TResponse, TNewResponse> To<TNewResponse>(string newDescription,
        Func<TResponse, TNewResponse> passThroughFunction
    );

    TResponse? GetTypedValue();
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

    public TResponse? GetTypedValue()
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
                throw new Exception("Invalid Data Type.");
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

    public TResponse? GetTypedValue()
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
}
