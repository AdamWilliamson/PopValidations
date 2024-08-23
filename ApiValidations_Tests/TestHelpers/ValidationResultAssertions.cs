﻿using ApiValidations.Descriptors.Core;
using ApiValidations.Helpers;
using ApiValidations_Tests.ValidationsTests.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using PopValidations.Execution.Validation;
using PopValidations.Validations.Base;
using System.Linq.Expressions;
using System.Reflection;

namespace PopValidations_Tests.TestHelpers;

public class ValidationResultAssertions :
    ReferenceTypeAssertions<ValidationResult, ValidationResultAssertions>
{
    public ValidationResultAssertions(ValidationResult instance)
        : base(instance)
    {}

    protected override string Identifier => nameof(ValidationResult);

    [CustomAssertion]
    public AndConstraint<ValidationResultAssertions> ContainsParam(
        LambdaExpression expression,
        int paramIndex,
        //string validationName,
        string message,
        //(string Key, string Value)[]? keyValuePairs,
        string because = "",
        params object[] becauseArgs
    )
    {
        var methodInfo = SymbolExtensions.GetMethodInfo(expression);
        var paramsList = methodInfo.GetParameters().Select(x => new ParamDetailsDTO(x.Name, x.ParameterType, x.Position)).ToList();
        var paramSelected = paramsList[paramIndex];
        var Name = methodInfo.Name
            + $"({string.Join(',', paramsList?.Select(x => GenericNameHelper.GetNameWithoutGenericArity(x.ParamType)) ?? [])})->"
            + GenericNameHelper.GetNameWithoutGenericArity(methodInfo.ReturnType)
            + ":"
            + $"Param({paramIndex},{GenericNameHelper.GetNameWithoutGenericArity(paramSelected.ParamType)},{paramSelected.Name})";
        return Contains(
            Name,
            //validationName,
            message,
            //keyValuePairs,
            because,
            becauseArgs
        );
    }

    [CustomAssertion]
    public AndConstraint<ValidationResultAssertions> ContainsParam(
        MethodInfo methodInfo,
        int paramIndex,
        //string validationName,
        string message,
        //(string Key, string Value)[]? keyValuePairs,
        string because = "",
        params object[] becauseArgs
    )
    {
        var paramsList = methodInfo.GetParameters().Select(x => new ParamDetailsDTO(x.Name, x.ParameterType, x.Position)).ToList();
        var paramSelected = paramsList[paramIndex];
        var Name = methodInfo.Name
            + $"({string.Join(',', paramsList?.Select(x => GenericNameHelper.GetNameWithoutGenericArity(x.ParamType)) ?? [])})->"
            + GenericNameHelper.GetNameWithoutGenericArity(methodInfo.ReturnType)
            + ":"
            + $"Param({paramIndex},{GenericNameHelper.GetNameWithoutGenericArity(paramSelected.ParamType)},{paramSelected.Name})";
        return Contains(
            Name,
            //validationName,
            message,
            //keyValuePairs,
            because,
            becauseArgs
        );
    }

    [CustomAssertion]
    public AndConstraint<ValidationResultAssertions> Contains(
        string property,
        //string validationName,
        string message,
        //(string Key, string Value)[]? keyValuePairs,
        string because = "",
        params object[] becauseArgs
    )
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .ForCondition(!string.IsNullOrEmpty(property))
            .FailWith("Property is required to find matching description")
            .Then
            .Given(() => Subject.Errors)
            .ForCondition(results => results.Any(x => x.Key.Contains(property)))
            .FailWith("Missing property {0}", property)
            //.Then
            //.Given((results) => results.SingleOrDefault(x => x.Key == property).Value)
            //.ForCondition(outcomes => outcomes != null && outcomes.Any(x => x.Validator == validationName))
            //.FailWith("Could not find Validator {0}", validationName)
            //.Then
            //.Given(outcomes => outcomes?.Where(x => x.Validator == validationName))
            //.ForCondition(
            //    outcomes => keyValuePairs == null
            //        || (
            //            outcomes?.Any(x => x.Values.All(x => keyValuePairs.Any(kvp => kvp.Key == x.Key && kvp.Value == x.Value)))
            //            ?? false
            //            )
            //)
            //.FailWith("No validator of name {0}, has a match for all properties", validationName)
            .Then
            .Given(outcomes => outcomes)
            .ForCondition(outcomes => outcomes?.Any(x => x.Value.Contains(message)) ?? false)
            .FailWith("Message {0} was not found", message);

        return new AndConstraint<ValidationResultAssertions>(this);
    }

    //[CustomAssertion]
    //public AndConstraint<ValidationResultAssertions> Matches(
    //    string message, 
    //    List<KeyValuePair<string, string>> keyValuePairs,
    //    string because = "", 
    //    params object[] becauseArgs
    //)
    //{
    //    Execute.Assertion
    //        .BecauseOf(because, becauseArgs)
    //        .ForCondition(Subject.Message == message)
    //        .FailWith(
    //            $"Expected {{context:{nameof(ValidationResult)}}} to contain {{0}}{{reason}}, but found {{1}}.",
    //            message, 
    //            Subject.Message)
    //        .Then
    //        .Given(() => Subject.KeyValues)
    //        .ForCondition(kvp => kvp.All(x => keyValuePairs.Any(newkvp => newkvp.Key == x.Key && newkvp.Value == x.Value)))
    //        .FailWith(
    //            $"Expected {{context:{nameof(ValidationResult)}}} to contain {{0}}{{reason}}, but found {{1}}.",
    //            _ => message, 
    //            kvp => kvp);

    //    return new AndConstraint<ValidationResultAssertions>(this);
    //}
}

