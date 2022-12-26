using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Execution.Stores.Internal;

public class FieldExecutor
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
    }

    public void SetParent(FieldExecutor? newParent)
    {
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
    public IFieldDescriptorOutline FieldDescriptor { get; }

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
