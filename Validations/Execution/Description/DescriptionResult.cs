using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Stores;
using PopValidations.Execution.Validation;
using PopValidations.Execution.Validations;
using PopValidations.Scopes;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Description;

public class DescriptionResult
{
    public List<DescriptionItemResult> Results = new();

    public void AddItem(ExpandedItem item, DescribeActionResult outcome)
    {
        var scopes = new List<IParentScope>();

        var child = item.ScopeParent;
        while (child != null)
        {
            if (child.CurrentScope != null
                && !scopes.Any(s => s.Id == child.CurrentScope.Id))
            {
                scopes.Add(child.CurrentScope);
            }
            child = child.PreviousScope;
        }

        var existingProperty = Results.SingleOrDefault(r => r.Property == item.PropertyName);
        var newOutcome = new DescriptionOutcome(item.PropertyName, outcome.Message);
        if (existingProperty != null)
        {
            existingProperty.Outcomes.Add(newOutcome);
        }
        else
        {
            existingProperty = new DescriptionItemResult(item.PropertyName);
            existingProperty.Outcomes.Add(newOutcome);

            Results.Add(existingProperty);
        }

        if (scopes.Count > 0)
        {
            ValidationGroupResult? group = null;
            foreach (var scope in scopes)
            {
                group = new ValidationGroupResult(scope, group);
            }
            newOutcome.Group = group;
        }
    }
}
