using PopValidations.Execution.Stores;
using PopValidations.Scopes;
using PopValidations.ValidatorInternals;

namespace PopValidations.Execution;

public abstract class PassThroughValidator<TValidationType> : AbstractValidatorBase<TValidationType>
{
    protected PassThroughValidator(IParentScope? parent, ValidationConstructionStore store) : base(parent, store) {}
}