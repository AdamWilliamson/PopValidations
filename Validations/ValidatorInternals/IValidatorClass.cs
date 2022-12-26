using PopValidations.Execution.Stores;
using PopValidations.Scopes;

namespace PopValidations.ValidatorInternals;

public interface IValidatorClass : IParentScope { }

public interface IMainValidator<TValidationType> : IValidatorClass
{
    ValidationConstructionStore Store { get; }
}

public interface ISubValidatorClass : IParentScope, IExpandableEntity
{
}