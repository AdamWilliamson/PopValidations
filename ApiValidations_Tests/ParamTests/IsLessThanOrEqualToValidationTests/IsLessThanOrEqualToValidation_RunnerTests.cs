using ApiValidations;
using ApiValidations.Execution;
using ApiValidations_Tests.TestHelpers;
using ApprovalTests;
using FluentAssertions;

namespace ApiValidations_Tests.ParamTests.IsLessThanOrEqualToValidationTests;

public class IsLessThanOrEqualtoApi
{
    public void SetValues(int param1, string param2, decimal param3, double param4, short param5, long param6) { }
    public void SetValue_Custom(int param1) { }
}

public class IsLessThanOrEqualTo_TestingValidator : ApiValidator<IsLessThanOrEqualtoApi>
{
    public IsLessThanOrEqualTo_TestingValidator()
    {
        DescribeFunc(x => x.SetValues(
            Param.Is<int>().IsLessThanOrEqualTo(int.MaxValue-1),
            Param.Is<string>().IsLessThanOrEqualTo(new string('a', 100)),
            Param.Is<decimal>().IsLessThanOrEqualTo(decimal.MaxValue - 1),
            Param.Is<double>().IsLessThanOrEqualTo(99999999.9999999),
            Param.Is<short>().IsLessThanOrEqualTo(short.MaxValue - 1),
            Param.Is<long>().IsLessThanOrEqualTo(long.MaxValue - 1)
        ));

        DescribeFunc(x => x.SetValue_Custom(
            Param.Is<int>().IsLessThanOrEqualTo(int.MaxValue-1, o => o
                .WithDescription("Make it less").WithErrorMessage("Not Less or Equal"))
        ));
    }
}

public class IsLessThanOrEqualToValidation_RunnerTests
{
    [Fact]
    public void GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(7);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLessThanOrEqualtoApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<IsLessThanOrEqualtoApi>(
            nameof(IsLessThanOrEqualtoApi.SetValues),
            0,
            [int.MaxValue,new string(char.MaxValue, 101), decimal.MaxValue, double.MaxValue, short.MaxValue, long.MaxValue],
            $"Is not less than or equal to '{int.MaxValue-1}'."
        );
        yield return new FunctionValidationTestDescription<IsLessThanOrEqualtoApi>(
            nameof(IsLessThanOrEqualtoApi.SetValue_Custom),
            0,
            [int.MaxValue],
            "Not Less or Equal"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<IsLessThanOrEqualtoApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_TestingValidator());

        // Act
        var results = await runner.Validate(
            new IsLessThanOrEqualtoApi(),
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

        using (ApprovalTestsHelpers.SilentForScenario($"{description.Function}_Param({description.ParamIndex})"))
        {
            Approvals.VerifyJson(JsonConverter.ToJson(results));
        }
    }

    [Fact]
    public async Task WhenValidating_ItSucceeds()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_TestingValidator());

        // Act
        var results = await runner.Validate(
            new IsLessThanOrEqualtoApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLessThanOrEqualtoApi).GetMethod(nameof(IsLessThanOrEqualtoApi.SetValue_Custom))!,
                [int.MaxValue-1]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
