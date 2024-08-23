using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;

namespace ApiValidations_Tests.ReturnTests.IsEnumToValidationTests;

public enum IsEnumTestEnum
{
    First = 10,
    Second = 20
}

public class EnumApi
{
    public IsEnumTestEnum IsEnum1() { return IsEnumTestEnum.First; }
    public string IsEnum2() { return string.Empty; }
    public int IsEnum3() { return 0; }
}

public class IsEnum_TestingValidator : ApiValidator<EnumApi>
{
    public IsEnum_TestingValidator()
    {
        DescribeFunc(x => x.IsEnum1()).Return.IsEnum(typeof(IsEnumTestEnum));
        DescribeFunc(x => x.IsEnum2()).Return.IsEnum(typeof(IsEnumTestEnum));
        DescribeFunc(x => x.IsEnum3()).Return.IsEnum(typeof(IsEnumTestEnum));
    }
}

public class IsEnumValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEnum_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(3);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EnumApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
