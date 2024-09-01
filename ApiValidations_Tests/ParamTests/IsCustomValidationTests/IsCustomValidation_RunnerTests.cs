using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.GenericTestableObjects;
using ApiValidations;
using PopValidations.Validations;
using FluentAssertions.Execution;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;
using ApprovalTests.Namers;
namespace PopValidations_Tests.ParamTests.IsCustomValidationTests;

public class IsCustom_TestingValidator : ApiValidator<BasicDataTypes>
{
    public IsCustom_TestingValidator()
    {
        var intParam1Validation = Param.Is<int>().Is("Must be min value.", "Is not min value.", x => x == int.MinValue);
        var decimalParam1Validation = Param.Is<decimal>().Is("Must be min value.", "Is not min value.", x => x == decimal.MinValue);
        var stringParam1Valdiation = Param.Is<string>().Is("Must be min value.", "Is not min value.", x => x == "aaaaa");

        DescribeFunc(x => x.NoReturnIntParam(intParam1Validation));
        DescribeFunc(x => x.NoReturnDecimalParam(decimalParam1Validation));
        DescribeFunc(x => x.NoReturnStringParam(stringParam1Valdiation));
    }
}

public class IsCustomValidation_RunnerTests
{
    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            validator: new IsCustom_TestingValidator()
        );

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        using var _ = new AssertionScope();

        description.Should().ContainsParam(
            (BasicDataTypes x) => x.NoReturnIntParam(FakeParam.Is<int>()),
            0,
            nameof(IsCustomValidation<int>),
            "Must be min value.",
            null
        );
        description.Results.Should().HaveCount(3);
        Approvals.VerifyJson(json);
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new object[] { typeof(BasicDataTypes), nameof(BasicDataTypes.NoReturnIntParam), int.MaxValue };
        yield return new object[] { typeof(BasicDataTypes), nameof(BasicDataTypes.NoReturnDecimalParam), decimal.MaxValue };
        yield return new object[] { typeof(BasicDataTypes), nameof(BasicDataTypes.NoReturnStringParam), "bbbb" };
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(Type apiType, string function, object value)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsCustom_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new BasicDataTypes(),
            new HeirarchyMethodInfo(
                string.Empty,
                apiType.GetMethod(function)!,
                [value]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(1);
        results.Should().ContainsParam(
            apiType.GetMethod(function)!,
            0,
            "Is not min value."
        );

        using (ApprovalResults.ForScenario(function))
        {
            Approvals.VerifyJson(JsonConverter.ToJson(results));
        }
    }

    public static IEnumerable<object[]> SuccessfulValues()
    {
        yield return new object[] { typeof(BasicDataTypes), nameof(BasicDataTypes.NoReturnIntParam), int.MinValue };
        yield return new object[] { typeof(BasicDataTypes), nameof(BasicDataTypes.NoReturnDecimalParam), decimal.MinValue };
        yield return new object[] { typeof(BasicDataTypes), nameof(BasicDataTypes.NoReturnStringParam), "aaaaa" };
    }

    [Theory]
    [MemberData(nameof(SuccessfulValues))]
    public async Task WhenValidating_ItSucceeds(Type apiType, string function, object value)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsCustom_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new BasicDataTypes(),
            new HeirarchyMethodInfo(
                string.Empty,
                apiType.GetMethod(function)!,
                [value]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
