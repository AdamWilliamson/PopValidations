using ApiValidations.Execution;
using PopValidations.Execution.Stores;
using PopValidations.ValidatorInternals;

namespace ApiValidations.Descriptors.Core;

public interface IParamVisitor
{
    ParamDetailsDTO? GetCurrentParamDescriptor();
    IValidationStore GetStore();
    IFunctionExpressionToken? GetCurrentFunction();
    bool IsRunningFunctionValidation();
    object? GetParamValue(int paramIndex);
    void SetCurrentExecutionContext(HeirarchyMethodInfo methodInfo);
}

public class ParamVisitor<TValidationType> : IParamVisitor
{
    private readonly IValidator owner;
    private readonly ParamValidationSetBuilder<TValidationType> builder;
    private HeirarchyMethodInfo? methodInfo;

    public ParamVisitor(IValidator owner, ParamValidationSetBuilder<TValidationType> builder)
    {
        this.owner = owner;
        this.builder = builder;
    }

    public ParamDetailsDTO? GetCurrentParamDescriptor() => owner.GetCurrentParamDescriptor();
    public IFunctionExpressionToken? GetTypedCurrentFunction() => builder.CurrentFunction;
    public IFunctionExpressionToken? GetCurrentFunction() => builder.CurrentFunction;
    public IValidationStore GetStore()
    {
        if (owner is IMainValidator<TValidationType> mainValidator)
        {
            return mainValidator.Store;
        }
        else if (owner is IStoreContainer storeOwner)
        {
            return storeOwner.Store;
        }
        else throw new Exception("Invalid Container Type");
    }

    public bool IsRunningFunctionValidation()
    {
        return methodInfo != null;
    }

    public object? GetParamValue(int paramIndex)
    {
        if (methodInfo is null) return null;

        return methodInfo.ParamValues[paramIndex];
    }

    public void SetCurrentExecutionContext(HeirarchyMethodInfo methodInfo)
    {
        this.methodInfo = methodInfo;
    }
}
