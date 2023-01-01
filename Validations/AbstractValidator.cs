using PopValidations.Scopes;
using PopValidations.ValidatorInternals;

namespace PopValidations;

public abstract class AbstractValidator<TValidationType> 
    : AbstractValidatorBase<TValidationType>, IMainValidator<TValidationType>
{
    public override string Name => typeof(TValidationType).Name;
    protected AbstractValidator(IParentScope? parent = null) : base(parent,new()) {}
}