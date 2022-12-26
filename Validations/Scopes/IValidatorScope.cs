using PopValidations.Execution.Stores;

namespace PopValidations.Scopes;

public interface IValidatorScope : IParentScope
{
    void ExpandToValidate(ValidationConstructionStore store, object? value);
}