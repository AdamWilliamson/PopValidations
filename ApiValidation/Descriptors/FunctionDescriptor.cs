using ApiValidations.Descriptors.Core;
using PopValidations.Execution.Stores;
using System.Diagnostics;

namespace ApiValidations.Descriptors;

public interface IFunctionDescriptor : IFunctionDescriptor_Internal
{
    IReturnDescriptor Return { get; }
}

public interface IFunctionDescriptor<TReturnType> : IFunctionDescriptor_Internal
{
    IReturnDescriptor<TReturnType> Return { get; }
}

public interface IEnumerableFunctionDescriptor<TReturnType> : IFunctionDescriptor_Internal
{
    IReturnDescriptor<IEnumerable<TReturnType>> Return { get; }
}

public class FunctionDescriptor<TValidationType>: IFunctionDescriptor
{
    private readonly IValidationStore store;
    protected IFunctionExpressionToken internalFunctionPropertyToken;

    public FunctionDescriptor(IFunctionExpressionToken functionPropertyToken, IValidationStore store)
    {
        Debug.Assert(functionPropertyToken != null);
        Debug.Assert(store != null);

        internalFunctionPropertyToken = functionPropertyToken;
        this.store = store;
    }

    public string Name => internalFunctionPropertyToken.Name;

    public IEnumerable<ParamDetailsDTO>? ParamList => internalFunctionPropertyToken?.Params;

    public Type? ReturnType => internalFunctionPropertyToken.ReturnType;

    public IReturnDescriptor Return => new ReturnDescriptor<TValidationType>(store, internalFunctionPropertyToken, new FunctionContext(ParamList?.Count() ?? 0));
}

public class FunctionDescriptor<TValidationType, TReturnType>: IFunctionDescriptor<TReturnType>
{
    private readonly IValidationStore store;
    protected IFunctionExpressionToken internalFunctionPropertyToken;

    public FunctionDescriptor(IFunctionExpressionToken functionPropertyToken, IValidationStore store)
    {
        Debug.Assert(functionPropertyToken != null);
        Debug.Assert(store != null);

        internalFunctionPropertyToken = functionPropertyToken;
        this.store = store;
    }

    public string Name => internalFunctionPropertyToken.Name;

    public IEnumerable<ParamDetailsDTO>? ParamList => internalFunctionPropertyToken?.Params;

    public Type? ReturnType => internalFunctionPropertyToken.ReturnType;

    public IReturnDescriptor<TReturnType> Return => new ReturnDescriptor<TReturnType, TValidationType>(store, internalFunctionPropertyToken, new FunctionContext(ParamList?.Count() ?? 0));
}