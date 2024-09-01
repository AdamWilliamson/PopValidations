using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;

namespace PopValidations_Tests.ParamTests.IsLessThanValidationTests;

public class IsLessThanApi
{
    public void Set(int param1) { }
    public void Set_Custom(int param1) { }
}

public class IsLessThan_TestingValidator : ApiValidator<IsLessThanApi>
{
    public IsLessThan_TestingValidator()
    {
        DescribeFunc(x => x.Set(Param.Is<int>().IsLessThan(int.MaxValue-1)));
        DescribeFunc(x => x.Set_Custom(Param.Is<int>().IsLessThan(int.MaxValue-1, o=> o.WithDescription("Be more").WithErrorMessage("Not more"))));
    }
}

public class IsLessThanValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLessThanApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<IsLessThanApi>(
            nameof(IsLessThanApi.Set),
            0,
            [int.MaxValue],
            $"Is not less than '{int.MaxValue - 1}'."
        );
        yield return new FunctionValidationTestDescription<IsLessThanApi>(
            nameof(IsLessThanApi.Set_Custom),
            0,
            [int.MaxValue],
            "Not more"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<IsLessThanApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsLessThanApi(),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThan_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsLessThanApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLessThanApi).GetMethod(nameof(IsLessThanApi.Set_Custom))!,
                [0]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
