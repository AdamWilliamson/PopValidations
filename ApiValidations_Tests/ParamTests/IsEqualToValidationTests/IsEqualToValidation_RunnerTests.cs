using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ValidationsTests.IsEqualToValidationTests;

public class EqualToApi
{
    public void SetInteger(int value) { }
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
        description.Results.Should().HaveCount(9);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EqualToApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
