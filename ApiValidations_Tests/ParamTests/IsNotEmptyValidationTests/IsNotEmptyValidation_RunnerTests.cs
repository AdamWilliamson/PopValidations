using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ValidationsTests;

public class NotEmptyApi 
{
    public void Set1(int? param1, string param2, decimal? param3, double? param4) { }
    public void Set2(short? param1, long? param2, object param3) { }
}

public class NotEmpty_TestingValidator : ApiValidator<NotEmptyApi>
{
    public NotEmpty_TestingValidator()
    {
        DescribeFunc(x => x.Set1(
            Param.Is<int?>().IsNotEmpty(),
            Param.Is<string>().IsEmpty(),
            Param.Is<decimal?>().IsEmpty(),
            Param.Is<double?>().IsEmpty()
        ));

        DescribeFunc(x => x.Set2(
            Param.Is<short?>().IsNotEmpty(),
            Param.Is<long?>().IsEmpty(),
            Param.Is<object>().IsEmpty()
        ));
    }
}

public class IsNotEmptyValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new NotEmpty_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(3);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<NotEmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
