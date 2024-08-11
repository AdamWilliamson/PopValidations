using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ValidationsTests.IsLengthExclusivelyBetweenValidationTests;

public class IsLengthExclusivelyBetweenApi
{
    public void Set(string param1, Array param2, IList<string> param3, LinkedList<int> param4, IEnumerable<double> param5, Dictionary<string, int> param6) { }
}

public class IsLengthExclusivelyBetween_TestingValidator : ApiValidator<IsLengthExclusivelyBetweenApi>
{
    public IsLengthExclusivelyBetween_TestingValidator()
    {
        DescribeFunc(x => x.Set(
            Param.Is<string>().IsLengthExclusivelyBetween(0, 10),
            Param.Is<Array>().IsLengthExclusivelyBetween(0, 10),
            Param.IsEnumerable<string>().IsLengthExclusivelyBetween(-1, 1).Convert<IList<string>>(),
            Param.Is<LinkedList<int>>().IsLengthExclusivelyBetween(-1, 1),
            Param.IsEnumerable<double>().IsLengthExclusivelyBetween(-1, 1).Convert<IEnumerable<double>>(),
            Param.Is<Dictionary<string, int>>().IsLengthExclusivelyBetween(-1, 1)
        ));
    }
}

public class IsLengthExclusivelyBetweenValidation_RunnerTests2
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsLengthExclusivelyBetween_TestingValidator()
        );

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(6);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLengthExclusivelyBetweenApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
