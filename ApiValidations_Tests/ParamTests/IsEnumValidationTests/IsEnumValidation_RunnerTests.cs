using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;
using ApiValidations.Execution;
using ApprovalTests.Namers;

namespace ApiValidations_Tests.ParamTests.IsEnumToValidationTests;

public enum IsEnumTestEnum
{
    First = 10,
    Second = 20
}

public class EnumApi
{
    public void IsEnum(IsEnumTestEnum param1, string param2, int param3) { }
    public void IsEnum_Custom(string param1) { }
}

public class IsEnum_TestingValidator : ApiValidator<EnumApi>
{
    public IsEnum_TestingValidator()
    {
        DescribeFunc(x => x.IsEnum(
            Param.Is<IsEnumTestEnum>().IsEnum(typeof(IsEnumTestEnum)),
            Param.Is<string>().IsEnum(typeof(IsEnumTestEnum)),
            Param.Is<int>().IsEnum(typeof(IsEnumTestEnum))
        ));

        DescribeFunc(x => x.IsEnum_Custom(
            Param.Is<string>().IsEnum(typeof(IsEnumTestEnum), o => o.WithDescription("Keep it enum").WithErrorMessage("You didn't keep it enum."))
        ));
    }
}

public class IsEnumValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(4);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EnumApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<EnumApi>(
            nameof(EnumApi.IsEnum),
            0,
            [null, "NotAnEnumValue" , int.MaxValue],
            "'null' Is not a valid value."
        );
        yield return new FunctionValidationTestDescription<EnumApi>(
            nameof(EnumApi.IsEnum_Custom),
            0,
            ["Not Empty"],
            "You didn't keep it enum."
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<EnumApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new EnumApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                description.ApiType.GetMethod(description.Function)!,
                description.ParamInputs.ToList()
            )
        );

        // Assert
        var methodInfo = description.ApiType.GetMethod(description.Function)!;
        results.Errors.Should().HaveCount(methodInfo.GetParameters().Count());
        results.Should().ContainsParam(
            methodInfo,
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new EnumApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(EnumApi).GetMethod(nameof(EnumApi.IsEnum))!,
                [IsEnumTestEnum.First, "First", 10]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
