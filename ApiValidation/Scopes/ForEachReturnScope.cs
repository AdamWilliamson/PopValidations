﻿using ApiValidations.Descriptors;
using ApiValidations.Descriptors.Core;
using PopValidations.Execution.Stores;
using PopValidations.Scopes;

namespace ApiValidations.Scopes;

internal class ForEachReturnScope<TReturnType> : ScopeBase
{
    private readonly IFunctionExpressionToken functionExpressionToken;
    private readonly IReturnDescriptor<IEnumerable<TReturnType>> returnDescriptor;
    private readonly Action<IReturnDescriptor<TReturnType>> actions;

    public override bool IgnoreScope => true;

    public ForEachReturnScope(
        IFunctionExpressionToken functionExpressionToken,
        IReturnDescriptor<IEnumerable<TReturnType>> returnDescriptor,
        Action<IReturnDescriptor<TReturnType>> actions
    )
    {
        this.functionExpressionToken = functionExpressionToken;
        this.returnDescriptor = returnDescriptor ?? throw new Exception("Wrong type of field descriptor");
        this.actions = actions;
    }

    public override string Name => nameof(ForEachReturnScope<TReturnType>);

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        throw new NotImplementedException();
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        var thingo = new ForEachReturnDescriptor<IEnumerable<TReturnType>, TReturnType>(
           store,
           functionExpressionToken,
           -1
        );

        actions.Invoke(thingo);
    }

    public override void ChangeStore(IValidationStore store) { }
}