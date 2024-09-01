
using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;

namespace ApiValidations_Tests.ParamTests.IsNotNullValidationTests;

public class IsNotNullApi
{
    public void Set1(int? param1, string param2, decimal? param3, double? param4) { }
    public void Set2(short? param1, long? param2, object param3) { }
    public void Set_Custom(object? param3) { }
}

public class IsNotNull_TestingValidator : ApiValidator<IsNotNullApi>
{
    public IsNotNull_TestingValidator()
    {
        DescribeFunc(x => x.Set1(
            Param.Is<int?>().IsNotNull(),
            Param.Is<string>().IsNotNull(),
            Param.Is<decimal?>().IsNotNull(),
            Param.Is<double?>().IsNotNull()
        ));

        DescribeFunc(x => x.Set2(
            Param.Is<short?>().IsNotNull(),
            Param.Is<long?>().IsNotNull(),
            Param.Is<object>().IsNotNull()
        ));

        DescribeFunc(x => x.Set_Custom(Param.Is<object?>().IsNotNull(o => o.WithDescription("Dont be Null").WithErrorMessage("Is Null"))));
    }
}

public class IsNotNullValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotNull_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(8);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsNotNullApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<IsNotNullApi>(
            nameof(IsNotNullApi.Set1),
            0,
            [null, null, null, null],
            $"Is null."
        );
        yield return new FunctionValidationTestDescription<IsNotNullApi>(
            nameof(IsNotNullApi.Set2),
            0,
            [null, null, null],
            $"Is null."
        );
        yield return new FunctionValidationTestDescription<IsNotNullApi>(
            nameof(IsNotNullApi.Set_Custom),
            0,
            [null],
            "Is Null"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<IsNotNullApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotNull_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsNotNullApi(),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotNull_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsNotNullApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(IsNotNullApi).GetMethod(nameof(IsNotNullApi.Set_Custom))!,
                [0]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
