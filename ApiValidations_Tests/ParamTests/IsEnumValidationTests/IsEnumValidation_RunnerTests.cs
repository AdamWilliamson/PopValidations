using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;
using ApiValidations_Tests.ValidationsTests.IsEmptyValidationTests.IsEmptyValidationTests;

namespace ApiValidations_Tests.ValidationsTests.IsEnumToValidationTests;

public enum IsEnumTestEnum
{
    First = 10,
    Second = 20
}

public class EnumApi
{
    public void IsEnum(IsEnumTestEnum param1, string param2, int param3) { }
}

public class IsEnum_TestingValidator : ApiValidator<EnumApi>
{
    public IsEnum_TestingValidator()
    {
        DescribeFunc(x => x.IsEnum(
            Param.Is<IsEnumTestEnum>().IsEnum(typeof(IsEnumTestEnum)),
            Param.Is<string>().IsEnum(typeof(IsEnumTestEnum)),
            Param.Is<int>().IsEnum(typeof(IsEnumTestEnum))
        ));
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
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
