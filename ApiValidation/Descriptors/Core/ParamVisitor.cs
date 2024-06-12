using PopValidations.Execution.Stores;
using PopValidations.ValidatorInternals;

namespace ApiValidations.Descriptors.Core;

public interface IParamVisitor
{
    ParamDetailsDTO? GetCurrentParamDescriptor();
    IValidationStore GetStore();
    IFunctionExpressionToken? GetCurrentFunction();
}

public class ParamVisitor<TValidationType> : IParamVisitor
{
    private readonly IValidator owner;
    private readonly ParamValidationSetBuilder<TValidationType> builder;

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
}
