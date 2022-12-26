using System;
using System.Collections.Generic;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes.ForEachs;

internal class ForEachIndexedScope<TValidationType, TFieldType> : ScopeBase
{
    public override bool IgnoreScope => true;
    private readonly FieldDescriptor<IEnumerable<TFieldType>, TFieldType> fieldDescriptor;
    private readonly Action<IFieldDescriptor<IEnumerable<TFieldType>, TFieldType>> actions;

    public override string Name => "Nothing";

    public ForEachIndexedScope(
        ValidationConstructionStore store,
        FieldDescriptor<IEnumerable<TFieldType>, TFieldType> fieldDescriptor,
        Action<IFieldDescriptor<IEnumerable<TFieldType>, TFieldType>> actions
    ) : base(store)
    {
        this.fieldDescriptor = fieldDescriptor;
        this.actions = actions;
    }

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        actions.Invoke(fieldDescriptor);
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        actions.Invoke(fieldDescriptor);
    }
}
