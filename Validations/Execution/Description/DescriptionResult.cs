using System.Collections.Generic;
using System.Linq;
using PopValidations.Execution.Stores;
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
        if (existingProperty == null)
        {
            existingProperty = new DescriptionItemResult(item.PropertyName);
            Results.Add(existingProperty);
        }


        if (scopes.Any())
        {
            DescriptionGroupResult? foundGroup = null;
            var tempScopes = scopes.ToList();
            tempScopes.Reverse();
            foreach (var scope in tempScopes)
            {
                DescriptionGroupResult? newfoundGroup = null;
                if (foundGroup == null)
                {
                    newfoundGroup = existingProperty.ValidationGroups.FirstOrDefault(f => f.Id == scope.Id);
                }
                else
                {
                    newfoundGroup = foundGroup.Children.FirstOrDefault(f => f.Id == scope.Id);
                }

                if (newfoundGroup == null)
                {
                    newfoundGroup = new DescriptionGroupResult(scope);
                    
                    if (foundGroup is not null)
                        foundGroup.Children.Add(newfoundGroup);
                    else if (scope == tempScopes.First())
                    {
                        existingProperty.ValidationGroups.Add(newfoundGroup);
                    }
                }

                foundGroup = newfoundGroup;

                if (scope == tempScopes.Last())
                {
                    foundGroup.Outcomes.Add(new DescriptionOutcome(outcome.Validator, outcome.Message, outcome.KeyValues));
                }
            }
        }
        else
        {
            existingProperty.Outcomes.Add(new DescriptionOutcome(outcome.Validator, outcome.Message, outcome.KeyValues));
        }
    }
}
