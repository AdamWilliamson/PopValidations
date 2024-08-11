
using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ValidationsTests.IsNotNullValidationTests;

public class IsNotNullApi
{
    public void Set1(int? param1, string param2, decimal? param3, double? param4) { }
    public void Set2(short? param1, long? param2, object param3) { }
}

public class IsNotNull_TestingValidator : ApiValidator<IsNotNullApi>
{
    public IsNotNull_TestingValidator()
    {
        DescribeFunc(x => x.Set1(
            Param.Is<int?>().IsNotNull(),
            Param.Is<string>().IsNotNull(),
            Param.Is<decimal?>().IsNotNull(),
            Param.Is<double?>().IsNotNull()
        ));

        DescribeFunc(x => x.Set2(
            Param.Is<short?>().IsNotNull(),
            Param.Is<long?>().IsNotNull(),
            Param.Is<object>().IsNotNull()
        ));
    }
}

public class IsNotNullValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotNull_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(7);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsNotNullApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
