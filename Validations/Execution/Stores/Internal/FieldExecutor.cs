using PopValidations.FieldDescriptors.Base;
using System.Collections.Generic;

namespace PopValidations.Execution.Stores.Internal;

public class FieldExecutor : IFieldDescriptorOutline
{
    object? RetrievedValue = null;
    bool ValueHasBeenRetrieved = false;

    public FieldExecutor(
        FieldExecutor? parent,
        IFieldDescriptorOutline fieldDescriptor
    )
    {
        Parent = parent;
        FieldDescriptor = fieldDescriptor;

        PrintName();
    }

    private void PrintName()
    {
        if (FieldDescriptor != null)
            DebugLogger.Log(FieldDescriptor.PropertyName);

       if (Parent != null)
            Parent.PrintName();
    }

    #region IFieldDescriptorOutline
    public string PropertyName 
    { 
        get 
        {
            if (FieldDescriptor != null && Parent != null)
                return FieldDescriptor.AddTo(Parent.PropertyName);
            else if (FieldDescriptor != null)
                return FieldDescriptor.PropertyName;
            else
                return string.Empty;
        } 
    }

    public void UpdateContext(Dictionary<string, object?> context)
    {
        FieldDescriptor?.UpdateContext(context);
    }

    public string AddTo(string existing) => FieldDescriptor?.AddTo(existing) ?? existing;
    #endregion


    public void SetParent(FieldExecutor? newParent)
    {
        if (newParent == this || newParent == null)
        {
            return;
        }

        if (Parent == null)
        {
            Parent = newParent;
        }
        else
        {
            Parent.SetParent(newParent);
        }
    }

    public FieldExecutor? Parent { get; protected set; }
    protected IFieldDescriptorOutline FieldDescriptor { get; init; }

    public object? GetValue(object? value)
    {
        if (ValueHasBeenRetrieved) return RetrievedValue;

        object? result;

        if (Parent != null)
        {
            result = FieldDescriptor.GetValue(Parent.GetValue(value));

        }
        else
            result = FieldDescriptor.GetValue(value);

        RetrievedValue = result;
        ValueHasBeenRetrieved = true;
        return result;
    }
}
