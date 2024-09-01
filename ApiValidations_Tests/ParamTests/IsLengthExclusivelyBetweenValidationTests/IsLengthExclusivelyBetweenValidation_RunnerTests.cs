using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApprovalTests.Namers;
using ApiValidations.Execution;

namespace ApiValidations_Tests.ParamTests.IsLengthExclusivelyBetweenValidationTests;

public class IsLengthExclusivelyBetweenApi
{
    public void Set(string param1, Array param2, IList<string> param3, LinkedList<int> param4, IEnumerable<double> param5, Dictionary<string, int> param6) { }
    public void Set_Custom(string param1) { }
}

public class IsLengthExclusivelyBetween_TestingValidator : ApiValidator<IsLengthExclusivelyBetweenApi>
{
    public IsLengthExclusivelyBetween_TestingValidator()
    {
        DescribeFunc(x => x.Set(
            Param.Is<string>().IsLengthExclusivelyBetween(0, 10),
            Param.Is<Array>().IsLengthExclusivelyBetween(10, 20),
            Param.IsEnumerable<string>().IsLengthExclusivelyBetween(-1, 0).Convert<IList<string>>(),
            Param.Is<LinkedList<int>>().IsLengthExclusivelyBetween(10, 15),
            Param.IsEnumerable<double>().IsLengthExclusivelyBetween(5, 7).Convert<IEnumerable<double>>(),
            Param.Is<Dictionary<string, int>>().IsLengthExclusivelyBetween(-1, 0)
        ));

        DescribeFunc(x => x.Set_Custom(
            Param.Is<string>().IsLengthExclusivelyBetween(0, 10, o => o.WithDescription("Keep inside the lines").WithErrorMessage("Outside the line"))
        ));
    }
}

public class IsLengthExclusivelyBetweenValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsLengthExclusivelyBetween_TestingValidator()
        );

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(7);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLengthExclusivelyBetweenApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<IsLengthExclusivelyBetweenApi>(
            nameof(IsLengthExclusivelyBetweenApi.Set),
            0,
            ["12345678910", new[] { new object() }, new List<string>() { "" }, new LinkedList<int>(new List<int>() { 1 }), new List<double>() { 1.0 }, new Dictionary<string, int>() { { "key", 1 } }],
            $"Is not between 0 and 10 exclusive."
        );
        yield return new FunctionValidationTestDescription<IsLengthExclusivelyBetweenApi>(
            nameof(IsLengthExclusivelyBetweenApi.Set_Custom),
            0,
            ["12345678910"],
            "Outside the line"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<IsLengthExclusivelyBetweenApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLengthExclusivelyBetween_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new IsLengthExclusivelyBetweenApi(),
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
        var results = await runner.ValidateAndExecute(
            new IsLengthExclusivelyBetweenApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(IsLengthExclusivelyBetweenApi).GetMethod(nameof(IsLengthExclusivelyBetweenApi.Set_Custom))!,
                ["123456789"]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
