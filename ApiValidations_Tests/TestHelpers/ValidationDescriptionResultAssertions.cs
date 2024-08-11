using ApiValidations.Descriptors.Core;
using ApiValidations.Helpers;
using ApiValidations_Tests.ValidationsTests.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using PopValidations.Execution.Description;
using System.Linq.Expressions;
using System.Reflection;

namespace ApiValidations_Tests.TestHelpers;

public class DescriptionResultAssertions :
    ReferenceTypeAssertions<DescriptionResult, DescriptionResultAssertions>
{
    public DescriptionResultAssertions(DescriptionResult instance)
        : base(instance)
    {}

    protected override string Identifier => nameof(DescriptionResult);

    public AndConstraint<DescriptionResultAssertions> ContainsParam(
        LambdaExpression expression,
        int paramIndex,
        string validationName,
        string message,
        (string Key, string Value)[]? keyValuePairs,
        string because = "",
        params object[] becauseArgs
    )
    {
        var methodInfo = SymbolExtensions.GetMethodInfo(expression);
        var paramsList = methodInfo.GetParameters().Select(x => new ParamDetailsDTO(x.Name, x.ParameterType, x.Position)).ToList();
        var paramSelected = paramsList[paramIndex];
        var Name = methodInfo.Name 
            + $"({string.Join(',', paramsList?.Select(x => x.ParamType.Name) ?? [])}):"
            + GenericNameHelper.GetNameWithoutGenericArity(methodInfo.ReturnType) 
            + "::"
            + $"({paramSelected.Name},{paramIndex},{paramSelected.ParamType.Name})";
        return Contains(
            Name,
            validationName,
            message,
            keyValuePairs,
            because,
            becauseArgs
        );
    }

    public AndConstraint<DescriptionResultAssertions> ContainsParam(
        MethodInfo methodInfo,
        int paramIndex,
        string validationName,
        string message,
        (string Key, string Value)[]? keyValuePairs,
        string because = "",
        params object[] becauseArgs
    )
    {
        var paramsList = methodInfo.GetParameters().Select(x => new ParamDetailsDTO(x.Name, x.ParameterType, x.Position)).ToList();
        var paramSelected = paramsList[paramIndex];
        var Name = methodInfo.Name
            + $"({string.Join(',', paramsList?.Select(x => x.ParamType.Name) ?? [])}):"
            + GenericNameHelper.GetNameWithoutGenericArity(methodInfo.ReturnType)
            + "::"
            + $"({paramSelected.Name},{paramIndex},{paramSelected.ParamType.Name})";
        return Contains(
            Name,
            validationName,
            message,
            keyValuePairs,
            because,
            becauseArgs
        );
    }

    public AndConstraint<DescriptionResultAssertions> Contains(
        string property,
        string validationName,
        string message,
        (string Key, string Value)[]? keyValuePairs,
        string because = "", 
        params object[] becauseArgs
    )
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrEmpty(property))
            .FailWith("Property is required to find matching description")
            .Then
            .Given(() => Subject.Results)
            .ForCondition(results => results.Any(x => x.Property == property))
            .FailWith("Missing property {0}", property)
            .Then
            .Given((results) => results.SingleOrDefault(x => x.Property == property)?.Outcomes)
            .ForCondition(outcomes => outcomes != null && outcomes.Any(x => x.Validator == validationName))
            .FailWith("Could not find Validator {0}", validationName)
            .Then
            .Given(outcomes => outcomes?.Where(x => x.Validator == validationName))
            .ForCondition(
                outcomes => keyValuePairs == null 
                    || (
                        outcomes?.Any(x => x.Values.All(x => keyValuePairs.Any(kvp => kvp.Key == x.Key && kvp.Value == x.Value))) 
                        ?? false
                        )
            )
            .FailWith("No validator of name {0}, has a match for all properties", validationName)
            .Then
            .Given(outcomes => outcomes)
            .ForCondition(outcomes => outcomes?.Any(x => x.Message == message) ?? false)
            .FailWith("Message {0} was not found", message);

        return new AndConstraint<DescriptionResultAssertions>(this);
    }
}

