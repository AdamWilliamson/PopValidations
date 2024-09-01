using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;
using ApiValidations.Execution;
using ApprovalTests.Namers;

namespace ApiValidations_Tests.ParamTests.IsGreaterThanOrEqualToValidationTests;

public class IsGreaterThanOrEqualtoApi
{
    public void SetValues(int param1, string param2, decimal param3, double param4, short param5, long param6) { }
    public void SetValue_Custom(int param1) { }
}

public class IsGreaterThanOrEqualTo_TestingValidator : ApiValidator<IsGreaterThanOrEqualtoApi>
{
    public IsGreaterThanOrEqualTo_TestingValidator()
    {
        DescribeFunc(x => x.SetValues(
            Param.Is<int>().IsGreaterThanOrEqualTo(int.MaxValue),
            Param.Is<string>().IsGreaterThanOrEqualTo(new string(char.MaxValue, 100)),
            Param.Is<decimal>().IsGreaterThanOrEqualTo(decimal.MaxValue),
            Param.Is<double>().IsGreaterThanOrEqualTo(double.MaxValue),
            Param.Is<short>().IsGreaterThanOrEqualTo(short.MaxValue),
            Param.Is<long>().IsGreaterThanOrEqualTo(long.MaxValue)
        ));

        DescribeFunc(x => x.SetValue_Custom(
            Param.Is<int>().IsGreaterThanOrEqualTo(
                int.MaxValue, 
                o => o
                    .WithDescription("Be Big")
                    .WithErrorMessage("Too Small")
            )
        ));
    }
}

public class IsGreaterThanOrEqualToValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(7);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsGreaterThanOrEqualtoApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<IsGreaterThanOrEqualtoApi>(
            nameof(IsGreaterThanOrEqualtoApi.SetValues),
            0,
            [10, "a", 10.0m, 10.0, 10, 10],
            $"Is not greater than or equal to '{int.MaxValue}'."
        );
        yield return new FunctionValidationTestDescription<IsGreaterThanOrEqualtoApi>(
            nameof(IsGreaterThanOrEqualtoApi.SetValue_Custom),
            0,
            [10],
            "Too Small"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<IsGreaterThanOrEqualtoApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsGreaterThanOrEqualtoApi(),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsGreaterThanOrEqualtoApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(IsGreaterThanOrEqualtoApi).GetMethod(nameof(IsGreaterThanOrEqualtoApi.SetValue_Custom))!,
                [int.MaxValue]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
