using PopValidations.FieldDescriptors.Base;
using System.Collections.Generic;

namespace PopValidations.Execution.Stores.Internal;

class ValidationFieldDescriptorOutline : IFieldDescriptorOutline
{
    private readonly IFieldDescriptorOutline outline;

    public ValidationFieldDescriptorOutline(
        string propertyName,
        IFieldDescriptorOutline outline
    )
    {
        PropertyName = propertyName;
        this.outline = outline;

        PrintName();
    }

    private void PrintName()
    {
        DebugLogger.Log(PropertyName);
    }

    public string PropertyName { get; }

    public void UpdateContext(Dictionary<string, object?> context)
    {
        outline.UpdateContext(context);
    }

    public string AddTo(string existing)
    {
        return existing + "." + PropertyName;
    }

    public object? GetValue(object? input)
    {
        return outline.GetValue(input);
    }
}