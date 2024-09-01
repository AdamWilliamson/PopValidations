using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;
using ApprovalTests.Namers;

namespace ApiValidations_Tests.ParamTests.IsEqualToValidationTests;

public class EqualToApi
{
    public void SetInteger(int value) { }
    public void SetInteger_Custom(int value) { }
    public void SetString(string value) { }
    public void SetBoolean(bool value) { }
    public void SetDouble(double value) { }
    public void SetDecimal(decimal value) { }
    public void SetFloat(float value) { }
    public void SetShort(short value) { }
    public void SetLong(long value) { }
    public void SetObject(object value) { }
}

public class EqualTo_TestingValidator : ApiValidator<EqualToApi>
{
    public EqualTo_TestingValidator()
    {
        DescribeFunc(x => x.SetInteger(Param.Is<int>().IsEqualTo(1)));
        DescribeFunc(x => x.SetString(Param.Is<string>().IsEqualTo(1)));
        DescribeFunc(x => x.SetBoolean(Param.Is<bool>().IsEqualTo(1)));
        DescribeFunc(x => x.SetDouble(Param.Is<double>().IsEqualTo(1)));
        DescribeFunc(x => x.SetDecimal(Param.Is<decimal>().IsEqualTo(1)));

        DescribeFunc(x => x.SetFloat(Param.Is<float>().IsEqualTo(1)));
        DescribeFunc(x => x.SetShort(Param.Is<short>().IsEqualTo(1)));
        DescribeFunc(x => x.SetLong(Param.Is<long>().IsEqualTo(1)));
        DescribeFunc(x => x.SetObject(Param.Is<object>().IsEqualTo(1)));

        DescribeFunc(x => x.SetInteger_Custom(Param.Is<int>().IsEqualTo(1, o => o.WithDescription("Must be 1").WithErrorMessage("Not 1"))));
    }
}

public class IsEqualToValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new EqualTo_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(10);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EqualToApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<EqualToApi>(
            nameof(EqualToApi.SetInteger),
            0,
            [10],
            "Is not equal to '1'."
        );
        yield return new FunctionValidationTestDescription<EqualToApi>(
            nameof(EqualToApi.SetInteger_Custom),
            0,
            [10],
            "Not 1"
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<EqualToApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new EqualTo_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new EqualToApi(),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new EqualTo_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new EqualToApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(EqualToApi).GetMethod(nameof(EqualToApi.SetInteger))!,
                [1]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
