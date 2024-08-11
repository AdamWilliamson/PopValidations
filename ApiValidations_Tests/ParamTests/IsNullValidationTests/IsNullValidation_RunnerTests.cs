
using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ValidationsTests.IsNullValidationTests;

public class IsNullApi
{
    public void Set1(int? param1, string param2, decimal? param3, double? param4) { }
    public void Set2(short? param1, long? param2, object param3) { }
}

public class IsNull_TestingValidator : ApiValidator<IsNullApi>
{
    public IsNull_TestingValidator()
    {
        DescribeFunc(x => x.Set1(
            Param.Is<int?>().IsNull(),
            Param.Is<string>().IsNull(),
            Param.Is<decimal?>().IsNull(),
            Param.Is<double?>().IsNull()
        ));

        DescribeFunc(x => x.Set2(
            Param.Is<short?>().IsNull(),
            Param.Is<long?>().IsNull(),
            Param.Is<object>().IsNull()
        ));
    }
}

public class IsNullValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNull_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(7);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsNullApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
