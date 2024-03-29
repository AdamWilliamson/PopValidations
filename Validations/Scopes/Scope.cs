﻿using System;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Validations.Base;

namespace PopValidations.Scopes.Whens;

public sealed class Scope<TScopedDataType> : ScopeBase
{
    private readonly IScopedData<TScopedDataType> scopedData;
    private readonly Action<IScopedData<TScopedDataType>> rules;

    public override bool IgnoreScope => true;
    public override string Name => scopedData.Describe();

    public Scope(
        IScopedData<TScopedDataType> scopedData,
        Action<IScopedData<TScopedDataType>> rules
    )
    {
        this.scopedData = scopedData;
        this.rules = rules;
    }

    public override void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline) 
    {
        scopedData.ReHome(fieldDescriptorOutline);
    }

    protected override async void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        if (FieldDescriptor != null)
            await scopedData.Init(FieldDescriptor.GetValue(value));
        else
            await scopedData.Init(value);

        rules.Invoke(scopedData);
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        rules.Invoke(scopedData);
    }

    public override void ChangeStore(IValidationStore store) { }    
}