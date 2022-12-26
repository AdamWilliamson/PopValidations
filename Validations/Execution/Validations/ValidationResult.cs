using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Stores;
using PopValidations.Scopes;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Validation;

public class ValidationResult
{
    public List<ValidationItemResult> Results = new();

    public void AddItem(ExpandedItem item, ValidationActionResult outcome)
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
        var realName = item.FullAddressableName;

        var existingProperty = Results.SingleOrDefault(r => r.Property == item.PropertyName);
        var newOutcome = new Validations.ValidationOutcome(item.PropertyName, outcome.Success, outcome.Message);
        if (existingProperty != null)
        {
            existingProperty.Outcomes.Add(newOutcome);
        }
        else
        {
            existingProperty = new ValidationItemResult(item.PropertyName);
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
