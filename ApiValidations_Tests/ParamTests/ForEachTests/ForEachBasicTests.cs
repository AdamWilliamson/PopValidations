using ApiValidations;
using ApprovalTests;
using ApiValidations_Tests.TestHelpers;
using ApiValidations_Tests.ValidationsTests;
using FluentAssertions;

namespace ApiValidations_Tests.ParamTests.ForEachTests;

public class EnumerableParamApi
{
    public void EnumerableIntReturn(IEnumerable<int> param1){}
}

public class EnumerableParamApiValidator : ApiValidator<EnumerableParamApi>
{
    public EnumerableParamApiValidator()
    {
        DescribeFunc(x => x.EnumerableIntReturn(Param.IsEnumerable<int>().ForEach(x => x.IsNotNull()).Convert<IEnumerable<int>>()));
    }
}

public class ForEachBasicTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new EnumerableParamApiValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(3);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<NotEmptyApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }
}
