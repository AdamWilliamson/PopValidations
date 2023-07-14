using PopValidations.Execution.Description;

namespace PopValidations.Swashbuckle_Tests.Helpers;

public static class DescriptionResultExtensions
{
    public static List<string> GetOutcomesForProperty(this DescriptionResult outcome, string property)
    {
        return outcome.Results
            .FirstOrDefault(x => x.Property == property)
            ?.Outcomes.Select(c => c.Message)
            .ToList();
    }
}
