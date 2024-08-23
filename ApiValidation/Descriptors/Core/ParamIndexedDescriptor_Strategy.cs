using System.Collections;
using System.Collections.Generic;

namespace ApiValidations.Descriptors.Core;

public class ParamIndexedDescriptor_Strategy<TValidationType, TEnumeratedParamType, TParamType>
    : IParamDescriptor_Strategy<TValidationType, TParamType>
    where TEnumeratedParamType : IEnumerable<TParamType>
{
    private readonly IParamIndexedToken<TValidationType, TEnumeratedParamType, TParamType> paramToken;

    public ParamIndexedDescriptor_Strategy(
        IParamIndexedToken<TValidationType, TEnumeratedParamType, TParamType> paramToken,
        int? enumerableIndex
    )
    {
        this.paramToken = paramToken ?? throw new ArgumentException(nameof(paramToken));
        EnumerableIndex = enumerableIndex;
    }

    public int? EnumerableIndex { get; }
    protected string EnumerableIndexString => (EnumerableIndex is null or < 0) ? "n" : EnumerableIndex.ToString() ?? "n";
    public int? ParamIndex { get; }
    public string PropertyName => (ParamToken?.FunctionToken?.Name ?? string.Empty) + $"::({ParamToken!.Name},{ParamIndex},{ParamToken!.ParamType.Name})[{EnumerableIndexString}]";
    public IParamToken<TParamType> ParamToken => paramToken;

    public string AddTo(string existing)
    {
        return ParamToken?.FunctionToken?.CombineWithParentProperty(existing) + $"::({ParamToken!.Name},{ParamIndex},{ParamToken!.ParamType.Name})[{EnumerableIndexString}]";
    }

    public IParamDescriptor_Strategy<TValidationType, TParamType> Clone()
    {
        return new ParamIndexedDescriptor_Strategy<TValidationType, TEnumeratedParamType, TParamType>(
            paramToken,
            ParamIndex
        );
    }

    private object? GetindexedValue(IEnumerator enumerator, int curindex, int endIndex)
    {
        if (curindex == endIndex)
        {
            return enumerator.Current;
        }

        enumerator.MoveNext();
        return GetindexedValue(enumerator, curindex++, endIndex);
    }

    public virtual object? GetValue(object? value)
    {
        //throw new NotImplementedException("Shouldn't get Param value through Non-Api Validation.");
        if (!ParamIndex.HasValue || !EnumerableIndex.HasValue) return null;

        var pValue = ParamToken.Visitor.GetParamValue(ParamIndex.Value);

        return pValue switch
        {
            IList l => l[EnumerableIndex.Value],
            IEnumerable e =>GetindexedValue(e.GetEnumerator(), 0, EnumerableIndex.Value),
            _ => throw new Exception("Unknown enumerable type")
        };
    }

    //public void SetParamDetails(string name, int index, IFunctionExpressionToken function)
    //{
    //    paramToken.SetParamDetails(name, index, function);
    //}
    public void Solidify() { ParamToken.Solidify(); }
}