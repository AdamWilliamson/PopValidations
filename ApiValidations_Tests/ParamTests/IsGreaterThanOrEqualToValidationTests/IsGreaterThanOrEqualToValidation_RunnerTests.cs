using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;

namespace ApiValidations_Tests.ValidationsTests.IsGreaterThanOrEqualToValidationTests;

public class IsGreaterThanOrEqualtoApi
{
    public void SetValues(int param1, string param2, decimal param3, double param4, short param5, long param6) { }
}

public class IsGreaterThanOrEqualTo_TestingValidator : ApiValidator<IsGreaterThanOrEqualtoApi>
{
    public IsGreaterThanOrEqualTo_TestingValidator()
    {
        DescribeFunc(x => x.SetValues(
            Param.Is<int>().IsGreaterThanOrEqualTo(int.MaxValue),
            Param.Is<string>().IsGreaterThanOrEqualTo(new string(char.MaxValue, 100)),
            Param.Is<decimal>().IsGreaterThanOrEqualTo(decimal.MaxValue),
            Param.Is<double>().IsGreaterThanOrEqualTo(double.MaxValue),
            Param.Is<short>().IsGreaterThanOrEqualTo(short.MaxValue),
            Param.Is<long>().IsGreaterThanOrEqualTo(long.MaxValue)
        ));
    }
}

public class IsGreaterThanOrEqualToValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsGreaterThanOrEqualTo_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(6);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsGreaterThanOrEqualtoApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
