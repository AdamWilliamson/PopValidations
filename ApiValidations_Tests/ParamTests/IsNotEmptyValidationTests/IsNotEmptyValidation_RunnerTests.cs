using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;

namespace ApiValidations_Tests.ParamTests.IsNotEmptyValidationTests;

public class NotEmptyApi 
{
    public void Set1(int? param1, string param2, decimal? param3, double? param4) { }
    public void Set2(short? param1, long? param2, object param3) { }
    public void Set_Custom(object? param3) { }
}

public class NotEmpty_TestingValidator : ApiValidator<NotEmptyApi>
{
    public NotEmpty_TestingValidator()
    {
        DescribeFunc(x => x.Set1(
            Param.Is<int?>().IsNotEmpty(),
            Param.Is<string>().IsNotEmpty(),
            Param.Is<decimal?>().IsNotEmpty(),
            Param.Is<double?>().IsNotEmpty()
        ));

        DescribeFunc(x => x.Set2(
            Param.Is<short?>().IsNotEmpty(),
            Param.Is<long?>().IsNotEmpty(),
            Param.Is<object>().IsNotEmpty()
        ));

        DescribeFunc(x => x.Set_Custom(Param.Is<object?>().IsNotEmpty(o => o.WithDescription("Be Empty").WithErrorMessage("Why Empty"))));
    }
}

public class IsNotEmptyValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotEmpty_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(8);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<NotEmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<NotEmptyApi>(
            nameof(NotEmptyApi.Set1),
            0,
            [null, null, null, null],
            $"Is empty."
        );
        yield return new FunctionValidationTestDescription<NotEmptyApi>(
            nameof(NotEmptyApi.Set2),
            0,
            [null,null,null],
            $"Is empty."
        );
        yield return new FunctionValidationTestDescription<NotEmptyApi>(
            nameof(NotEmptyApi.Set_Custom),
            0,
            [null],
            "Why Empty"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<NotEmptyApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotEmpty_TestingValidator());

        // Act
        var results = await runner.Validate(
            new NotEmptyApi(),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotEmpty_TestingValidator());

        // Act
        var results = await runner.Validate(
            new NotEmptyApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(NotEmptyApi).GetMethod(nameof(NotEmptyApi.Set_Custom))!,
                [0]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
