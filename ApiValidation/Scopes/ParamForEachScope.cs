using ApiValidations.Descriptors;
using ApiValidations.Descriptors.Core;
using PopValidations.Execution.Stores;

namespace PopValidations.Scopes.ForEachs;

internal class ParamForEachScope<TValidationType, TListType, TParamType> : ScopeBase
    where TValidationType : class
    where TListType : IEnumerable<TParamType>
{
    private readonly IParamVisitor visitor;
    private readonly IParamDescriptor_Internal<TListType> paramDescriptor;
    private Action<ParamDescriptor<TParamType, TValidationType>> actions;

    public override bool IgnoreScope => true;

    public ParamForEachScope(
        IParamVisitor visitor,
        IParamDescriptor_Internal<TListType> paramDescriptor,
        Action<ParamDescriptor<TParamType, TValidationType>> actions
    )
    {
        this.visitor = visitor;
        this.paramDescriptor = paramDescriptor;
        this.actions = actions;
    }

    public override string Name => nameof(ParamForEachScope<TValidationType, TListType, TParamType>);

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        throw new NotImplementedException("Cannot invoke scope container in Non-Api validation.");
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        var pt = new ParamIndexedPropertyExpressionToken<TValidationType, TListType, TParamType>(
            visitor, -1);
                //    paramDescriptor.ParamToken,
                //    paramDescriptor.ParamToken.Index
                //);
        var thingo = new ParamDescriptor<TParamType, TValidationType>(
            visitor,
            new ParamIndexedDescriptor_Strategy<TValidationType, TListType, TParamType>(
                pt,
                -1
            )
        );

        actions.Invoke(thingo);
        thingo.Convert<string?>();
    }

    public override void ChangeStore(IValidationStore store) { }
}