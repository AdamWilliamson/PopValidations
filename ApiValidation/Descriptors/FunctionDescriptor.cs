using ApiValidations.Descriptors.Core;
using PopValidations.Execution.Stores;
using System.Diagnostics;

namespace ApiValidations.Descriptors;

public interface IFunctionDescriptor
{
    Type? ReturnType { get; }
    IReturnDescriptor Return { get; }
    string Name { get; }
    IEnumerable<ParamDetailsDTO>? ParamList { get; }
}

public interface IFunctionDescriptor<TReturnType> : IFunctionDescriptor
{
    IReturnDescriptor<TReturnType> TypedReturn { get; }
}

//public interface IFunctionDescriptionFor { 
//    public string? Name { get; }
//    IEnumerable<ParamDetailsDTO>? ParamList { get; }
//    Type? ReturnType { get; }
//}

//public interface IFunctionDescriptionFor<TValidationType> : IFunctionDescriptionFor
//{
//    IFunctionExpressionToken<TValidationType> FunctionPropertyToken { get; }
//    bool Matches(string name, Type returnType, IEnumerable<ParamDetailsDTO> paramList);
//}

//public abstract class FunctionDescriptorBase<TValidationType> : IFunctionDescriptionFor<TValidationType>
//{
//    protected IFunctionExpressionToken<TValidationType> internalFunctionProptertyToken;
//    IFunctionExpressionToken<TValidationType> IFunctionDescriptionFor<TValidationType>.FunctionPropertyToken => internalFunctionProptertyToken;

//    public string Name => internalFunctionProptertyToken.Name;

//    public IEnumerable<ParamDetailsDTO>? ParamList => internalFunctionProptertyToken?.Params;

//    public Type? ReturnType => internalFunctionProptertyToken?.ReturnType;

//    public FunctionDescriptorBase(IFunctionExpressionToken<TValidationType> functionPropertyToken)
//    {
//        internalFunctionProptertyToken = functionPropertyToken;
//    }


//}

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

    public IReturnDescriptor Return => new ReturnDescriptor<TValidationType>(store, internalFunctionPropertyToken);


    //public bool Matches(string name, Type returnType, IEnumerable<ParamDetailsDTO> paramList)
    //{
    //    return name == Name
    //        && returnType == ReturnType
    //        && (ParamList?.Select(p => p.ParamType).AsEnumerable().SequenceEqual(paramList.Select(p => p.ParamType).AsEnumerable()) ?? false)
    //        && (ParamList?.All(x => x.MatchesValue(x.ParamType)) ?? false);
    //}
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

    public IReturnDescriptor Return => new ReturnDescriptor<TValidationType>(store, internalFunctionPropertyToken);

    public IReturnDescriptor<TReturnType> TypedReturn => new ReturnDescriptor<TReturnType, TValidationType>(store, internalFunctionPropertyToken);

    //public bool Matches(string name, Type returnType, IEnumerable<ParamDetailsDTO> paramList)
    //{
    //    return name == Name
    //        && returnType == ReturnType
    //        && (ParamList?.Select(p => p.ParamType).AsEnumerable().SequenceEqual(paramList.Select(p => p.ParamType).AsEnumerable()) ?? false)
    //        && (ParamList?.All(x => x.MatchesValue(x.ParamType)) ?? false);
    //}
}