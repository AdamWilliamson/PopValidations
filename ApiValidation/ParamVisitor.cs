using PopValidations.Execution.Stores;
using PopValidations.ValidatorInternals;

namespace PopValidations_Functional_Testbed;

public class ParamVisitor<TValidationType>
{
    private readonly IValidator owner;
    private readonly ParamValidationSetBuilder<TValidationType> builder;

    public ParamVisitor(IValidator owner, ParamValidationSetBuilder<TValidationType> builder)
    {
        this.owner = owner;
        this.builder = builder;
    }

    public ParamDetailsDTO? GetCurrentParamDescriptor() => owner.GetCurrentParamDescriptor();
    public IFunctionDescriptionFor<TValidationType>? GetCurrentFunction() => builder.CurrentFunction;
    public IValidationStore GetStore() 
    {
        if (owner is IMainValidator<TValidationType> mainValidator)
        {
            return ((IMainValidator<TValidationType>)owner).Store;
        }
        else if (owner is IStoreContainer)
        {
            return ((IStoreContainer)owner).Store;
        }
        else throw new Exception("Invalid Container Type");
    }
}
