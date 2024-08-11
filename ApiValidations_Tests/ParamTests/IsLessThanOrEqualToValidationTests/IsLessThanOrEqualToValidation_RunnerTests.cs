using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApprovalTests;
using FluentAssertions;

namespace ApiValidations_Tests.ValidationsTests.IsLessThanOrEqualToValidationTests;

public class IsLessThanOrEqualtoApi
{
    public void SetValues(int param1, string param2, decimal param3, double param4, short param5, long param6) { }
}

public class IsLessThanOrEqualTo_TestingValidator : ApiValidator<IsLessThanOrEqualtoApi>
{
    public IsLessThanOrEqualTo_TestingValidator()
    {
        DescribeFunc(x => x.SetValues(
            Param.Is<int>().IsLessThanOrEqualTo(int.MaxValue),
            Param.Is<string>().IsLessThanOrEqualTo(new string(char.MaxValue, 100)),
            Param.Is<decimal>().IsLessThanOrEqualTo(decimal.MaxValue),
            Param.Is<double>().IsLessThanOrEqualTo(double.MaxValue),
            Param.Is<short>().IsLessThanOrEqualTo(short.MaxValue),
            Param.Is<long>().IsLessThanOrEqualTo(long.MaxValue)
        ));
    }
}

public class IsLessThanOrEqualToValidation_RunnerTests
{
    [Fact]
    public void GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsLessThanOrEqualTo_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(6);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLessThanOrEqualtoApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
