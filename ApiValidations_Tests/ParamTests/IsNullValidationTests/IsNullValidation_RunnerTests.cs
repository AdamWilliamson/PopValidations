
using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;

namespace ApiValidations_Tests.ParamTests.IsNullValidationTests;

public class IsNullApi
{
    public void Set1(int? param1, string param2, decimal? param3, double? param4) { }
    public void Set2(short? param1, long? param2, object param3) { }
    public void Set_Custom(object? param3) { }
}

public class IsNull_TestingValidator : ApiValidator<IsNullApi>
{
    public IsNull_TestingValidator()
    {
        DescribeFunc(x => x.Set1(
            Param.Is<int?>().IsNull(),
            Param.Is<string>().IsNull(),
            Param.Is<decimal?>().IsNull(),
            Param.Is<double?>().IsNull()
        ));

        DescribeFunc(x => x.Set2(
            Param.Is<short?>().IsNull(),
            Param.Is<long?>().IsNull(),
            Param.Is<object>().IsNull()
        ));

        DescribeFunc(x => x.Set_Custom(Param.Is<object?>().IsNull(o => o.WithDescription("Be null").WithErrorMessage("no"))));
    }
}

public class IsNullValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(8);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsNullApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<IsNullApi>(
            nameof(IsNullApi.Set1),
            0,
            [1, 1, 1, 1],
            $"Is not null."
        );
        yield return new FunctionValidationTestDescription<IsNullApi>(
            nameof(IsNullApi.Set2),
            0,
            [1, 1, new object()],
            $"Is not null."
        );
        yield return new FunctionValidationTestDescription<IsNullApi>(
            nameof(IsNullApi.Set_Custom),
            0,
            [new object()],
            "no"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<IsNullApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsNullApi(),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsNullApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(IsNullApi).GetMethod(nameof(IsNullApi.Set_Custom))!,
                [null]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
