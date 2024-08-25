using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Stores;
using PopValidations.Scopes;
using PopValidations.Validations.Base;

namespace PopValidations.Execution.Validation;

public class ValidationResult
{
    public Dictionary<string, List<string>> Errors { get; protected set; } = new ();

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

        var existingProperty = Errors.ContainsKey(item.PropertyName);
        var newOutcome = new Validations.ValidationOutcome(item.PropertyName, outcome.Success, outcome.Message);

        if (existingProperty)
        {
            if (Errors[item.PropertyName] == null)
            {
                Errors[item.PropertyName] = new();
            }

            if (!string.IsNullOrWhiteSpace(newOutcome.Message))
            {
                Errors[item.PropertyName].Add(newOutcome.Message);
            }
        }
        else
        {
            Errors.Add(item.PropertyName, new());
            if (!string.IsNullOrWhiteSpace(newOutcome.Message))
            {
                Errors[item.PropertyName].Add(newOutcome.Message);
            }
        }
    }
}
