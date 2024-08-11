using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ValidationsTests.IsGreaterThanValidationTests;

public class IsGreaterThanApi
{
    public void SetValues(int param1, string param2, decimal param3, double param4, short param5, long param6) { }
}

public class IsGreaterThan_TestingValidator : ApiValidator<IsGreaterThanApi>
{
    public IsGreaterThan_TestingValidator()
    {
        DescribeFunc(x => x.SetValues(
            Param.Is<int>().IsGreaterThan(int.MaxValue),
            Param.Is<string>().IsGreaterThan(new string(char.MaxValue, 100)),
            Param.Is<decimal>().IsGreaterThan(decimal.MaxValue),
            Param.Is<double>().IsGreaterThan(double.MaxValue),
            Param.Is<short>().IsGreaterThan(short.MaxValue),
            Param.Is<long>().IsGreaterThan(long.MaxValue)
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
        description.Results.Should().HaveCount(6);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsGreaterThanApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
