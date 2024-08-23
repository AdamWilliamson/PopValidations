using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ReturnTests.IsEqualToValidationTests;

public class EqualToApi
{
    public int GetInteger() { return 1; }
    public string GetString() { return string.Empty; }
    public bool GetBoolean() { return true; }
    public double GetDouble() { return 1; }
    public decimal GetDecimal() { return 1.0m; }
    public float GetFloat() { return 1.0f; }
    public short GetShort() { return 1; }
    public long GetLong() { return 1; }
    public object GetObject() { return new object(); }
}

public class EqualTo_TestingValidator : ApiValidator<EqualToApi>
{
    public EqualTo_TestingValidator()
    {
        DescribeFunc(x => x.GetInteger()).Return.IsEqualTo(1);
        DescribeFunc(x => x.GetString()).Return.IsEqualTo(1);
        DescribeFunc(x => x.GetBoolean()).Return.IsEqualTo(1);
        DescribeFunc(x => x.GetDouble()).Return.IsEqualTo(1);
        DescribeFunc(x => x.GetDecimal()).Return.IsEqualTo(1);

        DescribeFunc(x => x.GetFloat()).Return.IsEqualTo(1);
        DescribeFunc(x => x.GetShort()).Return.IsEqualTo(1);
        DescribeFunc(x => x.GetLong()).Return.IsEqualTo(1);
        DescribeFunc(x => x.GetObject()).Return.IsEqualTo(1);
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
