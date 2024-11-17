using PopValidations.Execution.Description;
using PopValidations.Execution.Validations;

namespace PopApiValidations.Swashbuckle.Internal.OperationFilter;

public record GroupedDescriptions(string GroupTitle, DescriptionOutcome Outcome);

public class ValidationForSchema
{
    public List<GroupedDescriptions> Outcomes { get; set; }
    public string PopApiObjectHeirarchy { get; set; }

    public static ValidationForSchema FromParamater(
        string parameterName,
        List<GroupedDescriptions> outcomes)
    {
        return new ValidationForSchema
        {
            PopApiObjectHeirarchy = parameterName,
            Outcomes = outcomes
        };
    }
}

public static class ValidationProcessor
{
    public static ValidationForSchema GetFlattenedValidationsFor(
        PopApiOpenApiConfig config,
        List<DescriptionItemResult> descriptionItems,
        //OpenApiParamNavigator navigator
        string parameterName
    )
    {
        var endOutcomes = new List<GroupedDescriptions>();

        if (descriptionItems?.Any() != true) return ValidationForSchema.FromParamater(parameterName, endOutcomes);

        foreach (var descriptionItem in descriptionItems.Where(x => x.Property.Equals(parameterName, StringComparison.OrdinalIgnoreCase)))
        {
            if (descriptionItem.Outcomes?.Any() == true)
            {
                foreach (var outcome in descriptionItem.Outcomes)
                {
                    if (outcome == null) continue;

                    endOutcomes.Add(new (string.Empty, outcome!));
                }
            }

            if (descriptionItem.ValidationGroups?.Any() == true)
            {
                foreach (var group in descriptionItem.ValidationGroups)
                {
                    endOutcomes.AddRange(FlattenRecurse(config, string.Empty, group));
                }
            }
        }

        return ValidationForSchema.FromParamater(parameterName, endOutcomes);
    }

    private static List<GroupedDescriptions> FlattenRecurse(
        PopApiOpenApiConfig config, 
        string existing, 
        DescriptionGroupResult group
    )
    {
        var endOutcomes = new List<GroupedDescriptions>();
        var additive = string.IsNullOrWhiteSpace(existing) ? group.Description : config.MultiGroupIndicator + group.Description;

        if (group.Outcomes?.Any() == true)
        {
            foreach (var outcome in group.Outcomes)
            {
                if (outcome == null) continue;

                endOutcomes.Add(new(existing + additive + config.GroupResultIndicator, outcome!));
            }
        }

        if (group.Children?.Any() == true)
        {
            foreach (var child in group.Children)
            {
                endOutcomes.AddRange(FlattenRecurse(config, existing + additive + config.GroupResultIndicator, child));
            }
        }

        return endOutcomes;
    }
}
