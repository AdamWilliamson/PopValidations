using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;

namespace ApiValidations_Tests.ParamTests.IsLengthInclusivelyBetweenValidationTests;

public class IsLengthInclusivelyBetweenApi
{
    public void Set(string param1, Array param2, IList<string> param3, LinkedList<int> param4, IEnumerable<double> param5, Dictionary<string, int> param6) { }
    public void Set_Custom(string param1) { }
}

public class IsLengthExclusivelyBetween_TestingValidator : ApiValidator<IsLengthInclusivelyBetweenApi>
{
    public IsLengthExclusivelyBetween_TestingValidator()
    {
        DescribeFunc(x => x.Set(
            Param.Is<string>().IsLengthInclusivelyBetween(0, 9),
            Param.Is<Array>().IsLengthInclusivelyBetween(5, 9),
            Param.IsEnumerable<string>().IsLengthInclusivelyBetween(-1, 0).Convert<IList<string>>(),
            Param.Is<LinkedList<int>>().IsLengthInclusivelyBetween(-1, 0),
            Param.IsEnumerable<double>().IsLengthInclusivelyBetween(-1, 0).Convert<IEnumerable<double>>(),
            Param.Is<Dictionary<string, int>>().IsLengthInclusivelyBetween(-1, 0)
        ));

        DescribeFunc(x => x.Set_Custom(
            Param.Is<string>()
                .IsLengthInclusivelyBetween(0, 10, o => o.WithDescription("Be Short").WithErrorMessage("Is not Short"))
        ));
    }
}

public class IsLengthInclusivelyBetweenValidation_RunnerTests
{
    [Fact]
    public void GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsLengthExclusivelyBetween_TestingValidator()
        );

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(7);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLengthInclusivelyBetweenApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<IsLengthInclusivelyBetweenApi>(
            nameof(IsLengthInclusivelyBetweenApi.Set),
            0,
            ["12345678910", new[] { new object() }, new List<string>() { "" }, new LinkedList<int>(new List<int>() { 1 }), new List<double>() { 1.0 }, new Dictionary<string, int>() { { "key", 1 } }],
            $"Is not between 0 and 9 inclusive."
        );
        yield return new FunctionValidationTestDescription<IsLengthInclusivelyBetweenApi>(
            nameof(IsLengthInclusivelyBetweenApi.Set_Custom),
            0,
            ["12345678910"],
            "Is not Short"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<IsLengthInclusivelyBetweenApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLengthExclusivelyBetween_TestingValidator());

        // Act
        var results = await runner.Validate(
            new IsLengthInclusivelyBetweenApi(),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLengthExclusivelyBetween_TestingValidator());

        // Act
        var results = await runner.Validate(
            new IsLengthInclusivelyBetweenApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLengthInclusivelyBetweenApi).GetMethod(nameof(IsLengthInclusivelyBetweenApi.Set_Custom))!,
                ["123456789"]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
