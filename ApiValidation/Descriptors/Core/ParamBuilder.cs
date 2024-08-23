using ApiValidations.Execution;

namespace ApiValidations.Descriptors.Core;

public class ParamBuilder<TValidationType>
{
    private readonly ParamVisitor<TValidationType> owner;

    public ParamBuilder(ParamVisitor<TValidationType> owner)
    {
        this.owner = owner;
    }

    public ParamDescriptor<TParamType, TValidationType> Is<TParamType>()
    {
        var pt = new ParamToken<TParamType, TValidationType>(owner);
        return new ParamDescriptor<TParamType, TValidationType>(
            owner,
            new ParamDescriptor_Strategy<TParamType, TValidationType>(
               pt 
            )
        );
    }

    public ParamDescriptor<IEnumerable<TParamType>, TValidationType> IsEnumerable<TParamType>()
    {
        var pt = new ParamToken<IEnumerable<TParamType>, TValidationType>(owner);
        return new ParamDescriptor<IEnumerable<TParamType>, TValidationType>(
            owner,
            new ParamDescriptor_Strategy<IEnumerable<TParamType>, TValidationType>(
                pt
            )
        );
    }

    public void SetCurrentExecutionContext(HeirarchyMethodInfo methodInfo)
    {
        owner.SetCurrentExecutionContext(methodInfo);
    }
}
