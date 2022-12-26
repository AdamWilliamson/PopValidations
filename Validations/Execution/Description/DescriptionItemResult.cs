using System.Collections.Generic;
using PopValidations.Execution.Validations;

namespace PopValidations.Execution.Description;

public class DescriptionItemResult
{
    public string Property { get; }
    public List<DescriptionOutcome> Outcomes { get; } = new();


    public DescriptionItemResult(string property)
    {
        Property = property;
    }
}
