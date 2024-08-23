using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;
using ApiValidations.Execution;
using ApprovalTests.Namers;
using System.Collections.Generic;

namespace ApiValidations_Tests.ParamTests.IsEmptyValidationTests.IsEmptyValidationTests;

public class EmptyApi
{
    public void AllParamsMustBeEmpty(string param1, List<string> param2, Dictionary<string,string> param3) { }
    public void AllParamsMustBeEmpty_Custom(string param1, List<string> param2, Dictionary<string, string> param3) { }
}

public class IsEmpty_TestingValidator : ApiValidator<EmptyApi>
{
    public IsEmpty_TestingValidator()
    {
        DescribeFunc(x => x.AllParamsMustBeEmpty(
            Param.Is<string>().IsEmpty(),
            Param.Is<List<string>>().IsEmpty(),
            Param.Is<Dictionary<string,string>>().IsEmpty()
        ));

        DescribeFunc(x => x.AllParamsMustBeEmpty_Custom(
            Param.Is<string>().IsEmpty(o => o.WithDescription("Be Empty.").WithErrorMessage("Was not Empty.")),
            Param.Is<List<string>>().IsEmpty(o => o.WithDescription("Be Empty.").WithErrorMessage("Was not Empty.")),
            Param.Is<Dictionary<string, string>>().IsEmpty(o => o.WithDescription("Be Empty.").WithErrorMessage("Was not Empty."))
        ));
    }
}

public class IsEmptyValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(6);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<EmptyApi>(
            nameof(EmptyApi.AllParamsMustBeEmpty), 
            0, 
            ["Not Empty", new List<string>() { "NotEmpty" }, new Dictionary<string, string>() { { "NotEmptyKey", "NotEmptyValue" } }],
            "Is not empty."
        );
        yield return new FunctionValidationTestDescription<EmptyApi>(
            nameof(EmptyApi.AllParamsMustBeEmpty_Custom), 
            0,
            ["Not Empty", new List<string>() { "NotEmpty" }, new Dictionary<string, string>() { { "NotEmptyKey", "NotEmptyValue" } }],
            "Was not Empty."
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<EmptyApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_TestingValidator());

        // Act
        var results = await runner.Validate(
            new EmptyApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                description.ApiType.GetMethod(description.Function)!,
                description.ParamInputs.ToList()
            )
        );

        // Assert
        results.Errors.Should().HaveCount(3);
        results.Should().ContainsParam(
            description.ApiType.GetMethod(description.Function)!,
            description.ParamIndex,
            description.Error
        );

        using (ApprovalResults.ForScenario($"{description.Function}_Param({description.ParamIndex})"))
        {
            Approvals.VerifyJson(JsonConverter.ToJson(results));
        }
    }


    [Fact]
    public async Task WhenValidating_ItSucceeds()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_TestingValidator());

        // Act
        var results = await runner.Validate(
            new EmptyApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(EmptyApi).GetMethod(nameof(EmptyApi.AllParamsMustBeEmpty))!,
                [string.Empty, null, null]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
