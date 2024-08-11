using ApprovalTests;
using FluentAssertions;
using ApiValidations_Tests.TestHelpers;
using ApiValidations;

namespace ApiValidations_Tests.ValidationsTests.IsEmptyValidationTests.IsEmptyValidationTests;

public class EmptyApi
{
    public void AllParamsMustBeEmpty(string param1, List<string> param2, Dictionary<string,string> param3) { }
}

public class IsEmpty_TestingValidator : ApiValidator<EmptyApi>
{
    public IsEmpty_TestingValidator()
    {
        DescribeFunc(x => x.AllParamsMustBeEmpty(
            Param.Is<string>().IsEmpty(),
            Param.Is<List<string>>().IsEmpty(),
            Param.Is<Dictionary<string,string>>().IsEmpty()
        ));
    }
}

public class IsEmptyValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmpty_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(3);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
