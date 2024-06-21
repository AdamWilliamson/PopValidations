using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using PopValidations.Validations.Base;

namespace PopValidations_Tests.TestHelpers;

public class ValidationActionResultAssertions :
    ReferenceTypeAssertions<ValidationActionResult, ValidationActionResultAssertions>
{
    public ValidationActionResultAssertions(ValidationActionResult instance)
        : base(instance)
    {}

    protected override string Identifier => nameof(ValidationActionResult);

    public AndConstraint<ValidationActionResultAssertions> Matches(
        string message, 
        List<KeyValuePair<string, string>> keyValuePairs,
        string because = "", 
        params object[] becauseArgs
    )
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(Subject.Message == message)
            .FailWith(
                $"Expected {{context:{nameof(ValidationActionResult)}}} to contain {{0}}{{reason}}, but found {{1}}.",
                message, 
                Subject.Message)
            .Then
            .Given(() => Subject.KeyValues)
            .ForCondition(kvp => kvp.All(x => keyValuePairs.Any(newkvp => newkvp.Key == x.Key && newkvp.Value == x.Value)))
            .FailWith(
                $"Expected {{context:{nameof(ValidationActionResult)}}} to contain {{0}}{{reason}}, but found {{1}}.",
                _ => message, 
                kvp => kvp);

        return new AndConstraint<ValidationActionResultAssertions>(this);
    }
}

