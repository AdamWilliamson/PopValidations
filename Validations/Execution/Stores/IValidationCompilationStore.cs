using System.Collections.Generic;

namespace PopValidations.Execution.Stores;

public interface IValidationCompilationStore
{
    List<ExpandedItem> Compile<TValidationType>(TValidationType? instance);
    List<ExpandedItem> Describe();
}
