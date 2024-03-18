using PopValidations.Execution.Stores;

namespace PopValidations_Functional_Testbed;

public interface IFunctionDescriptor
{
    IReturnDescriptor Return { get; }
}

public interface IFunctionDescriptor<TReturnType> 
{
    IReturnDescriptor<TReturnType> Return { get; }
}

public interface IFunctionDescriptorFor<TValidationType> {  }

public interface IFunctionDescriptionFor<TValidationType>
{
    IFunctionExpressionToken<TValidationType> FunctionPropertyToken { get; }
    bool Matches(string name, Type returnType, IEnumerable<ParamDetailsDTO> paramList);
}

public abstract class FunctionDescriptorBase<TValidationType> : IFunctionDescriptionFor<TValidationType>
{
    protected IFunctionExpressionToken<TValidationType> internalFunctionProptertyToken;
    IFunctionExpressionToken<TValidationType> IFunctionDescriptionFor<TValidationType>.FunctionPropertyToken => internalFunctionProptertyToken;

    public FunctionDescriptorBase(IFunctionExpressionToken<TValidationType> functionPropertyToken)
    {
        internalFunctionProptertyToken = functionPropertyToken;
    }

    bool IFunctionDescriptionFor<TValidationType>.Matches(string name, Type returnType, IEnumerable<ParamDetailsDTO> paramList)
    {
        return name == internalFunctionProptertyToken.Name
            && returnType == internalFunctionProptertyToken.ReturnType
            && Enumerable.SequenceEqual(internalFunctionProptertyToken.Params.Select(p => p.ParamType).AsEnumerable(), paramList.Select(p => p.ParamType).AsEnumerable())
            && internalFunctionProptertyToken.Params.All(x => x.MatchesValue(x.ParamType));
    }
}

public class FunctionDescriptor<TValidationType> : FunctionDescriptorBase<TValidationType>, IFunctionDescriptorFor<TValidationType>, IFunctionDescriptor
{
    private readonly IValidationStore store;

    public FunctionDescriptor(IFunctionExpressionToken<TValidationType> functionPropertyToken, IValidationStore store)
        :base(functionPropertyToken)
    {
        this.store = store;
    }

    public IReturnDescriptor Return => new ReturnDescriptor<TValidationType>(store, this);
}

public class FunctionDescriptor<TValidationType, TReturnType> : FunctionDescriptorBase<TValidationType>, IFunctionDescriptor<TReturnType>
{
    private readonly IValidationStore store;

    public FunctionDescriptor(IFunctionExpressionToken<TValidationType> functionPropertyToken, IValidationStore store)
        : base(functionPropertyToken)
    {
        this.store = store;
    }

    public IReturnDescriptor<TReturnType> Return => new ReturnDescriptor<TReturnType, TValidationType>(store, this);
}