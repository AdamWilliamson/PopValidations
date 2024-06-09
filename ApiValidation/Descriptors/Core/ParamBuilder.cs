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
        var pt = new ParamExpressionToken<TParamType, TValidationType>();
        return new ParamDescriptor<TParamType, TValidationType>(
            pt,
            owner,
            new ParamDescriptor_Strategy<TParamType, TValidationType>(
               pt 
            )
        );
    }

    public ParamDescriptor<IEnumerable<TParamType>, TValidationType> IsEnumerable<TParamType>()
    {
        var pt = new ParamExpressionToken<IEnumerable<TParamType>, TValidationType>();
        return new ParamDescriptor<IEnumerable<TParamType>, TValidationType>(
            pt,
            owner,
            new ParamDescriptor_Strategy<IEnumerable<TParamType>, TValidationType>(
                pt
            )
        );
    }
}
