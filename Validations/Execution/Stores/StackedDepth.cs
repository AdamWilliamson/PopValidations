using System;
using PopValidations.Execution.Stores.Internal;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Execution.Stores;

public class StackedDepth
{
    public Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? Decorator { get; init; }
    public ScopeParent? ScopeParent { get; init; }
    //public IFieldDescriptorOutline? FieldParent { get; init; }
    public FieldExecutor? FieldExecutor { get; init; }

    public StackedDepth(
        ScopeParent? scopeParent,
        //IFieldDescriptorOutline? fieldParent,
        FieldExecutor? fieldExecutor,
        Func<IValidatableStoreItem, IFieldDescriptorOutline?, IValidatableStoreItem>? decorator
        )
    {
        ScopeParent = scopeParent;
        //FieldParent = fieldParent;
        FieldExecutor = fieldExecutor;
        Decorator = decorator;
    }
}
