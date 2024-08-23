using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;
using ApprovalTests.Namers;

namespace ApiValidations_Tests.ParamTests.IsGreaterThanValidationTests;

public class IsGreaterThanApi
{
    public void SetValues(int param1, string param2, decimal param3, double param4, short param5, long param6) { }
    public void SetValue_Custom(int param1) { }
}

public class IsGreaterThan_TestingValidator : ApiValidator<IsGreaterThanApi>
{
    public IsGreaterThan_TestingValidator()
    {
        DescribeFunc(x => x.SetValues(
            Param.Is<int>().IsGreaterThan(int.MaxValue - 1),
            Param.Is<string>().IsGreaterThan(new string(char.MaxValue, 100)),
            Param.Is<decimal>().IsGreaterThan(decimal.MaxValue - 1),
            Param.Is<double>().IsGreaterThan(double.MaxValue - 1),
            Param.Is<short>().IsGreaterThan(short.MaxValue - 1),
            Param.Is<long>().IsGreaterThan(long.MaxValue - 1)
        ));

        DescribeFunc(x => x.SetValue_Custom(
            Param.Is<int>().IsGreaterThan(
                int.MaxValue-1, 
                o => o
                    .WithDescription("Be Big")
                    .WithErrorMessage("Too Small")
            )
        ));
    }
}

public class IsGreaterThanValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(7);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsGreaterThanApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<IsGreaterThanApi>(
            nameof(IsGreaterThanApi.SetValues),
            0,
            [10, "a", 10.0m, 10.0, 10, 10],
            $"Is not greater than '{int.MaxValue-1}'."
        );
        yield return new FunctionValidationTestDescription<IsGreaterThanApi>(
            nameof(IsGreaterThanApi.SetValue_Custom),
            0,
            [10],
            "Too Small"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<IsGreaterThanApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_TestingValidator());

        // Act
        var results = await runner.Validate(
            new IsGreaterThanApi(),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThan_TestingValidator());

        // Act
        var results = await runner.Validate(
            new IsGreaterThanApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(IsGreaterThanApi).GetMethod(nameof(IsGreaterThanApi.SetValue_Custom))!,
                [int.MaxValue]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
