using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ValidationsTests.IsLengthInclusivelyBetweenValidationTests;

public class IsLengthInclusivelyBetween
{
    public void Set(string param1, Array param2, IList<string> param3, LinkedList<int> param4, IEnumerable<double> param5, Dictionary<string, int> param6) { }
}

public class IsLengthExclusivelyBetween_TestingValidator : ApiValidator<IsLengthInclusivelyBetween>
{
    public IsLengthExclusivelyBetween_TestingValidator()
    {
        DescribeFunc(x => x.Set(
            Param.Is<string>().IsLengthInclusivelyBetween(0, 10),
            Param.Is<Array>().IsLengthInclusivelyBetween(0, 10),
            Param.IsEnumerable<string>().IsLengthInclusivelyBetween(-1, 1).Convert<IList<string>>(),
            Param.Is<LinkedList<int>>().IsLengthInclusivelyBetween(-1, 1),
            Param.IsEnumerable<double>().IsLengthInclusivelyBetween(-1, 1).Convert<IEnumerable<double>>(),
            Param.Is<Dictionary<string, int>>().IsLengthInclusivelyBetween(-1, 1)
        ));
    }
}

public class IsLengthInclusivelyBetweenValidation_RunnerTests
{
    [Fact]
    public void GivenAValidator_WithNoErrors_ThenNoErrorsAreProduced()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            new IsLengthExclusivelyBetween_TestingValidator()
        );

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(6);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsLengthInclusivelyBetween>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
