using ApprovalTests;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace ApiValidations_Tests.ParamTests.ScopeValidationTests.Scope_IncludeInsideScopeWhenValidationTests;

public class Validator : ApiValidator<Level1>
{
    public Validator()
    {
        Scope(
           "Database Value",
           (x) => DataRetriever.GetValue(x),
           (retrievedData) =>
           {
               DescribeFunc(x => x.Check(Param.Is<bool>().IsEqualTo(retrievedData)));

               Include(new SubValidator());
           }
        );
    }
}

public class SubValidator : ApiSubValidator<Level1>
{
    public SubValidator()
    {
        DescribeFunc(x => x.Check(Param.Is<bool>().IsEqualTo(true)));

        Scope(
            "Another DB Value",
            (x) => DataRetriever.GetMoreValue(x),
            (moreData) =>
            {
                DescribeFunc(x => x.Check(Param.Is<bool>().IsEqualTo(moreData)));
            }
        );
    }
}

public class Scope_IncludeInsideScopeTest
{
    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new Validator());

        // Act
        var descriptionResult = descriptionRunner.Describe();

        // Assert
        Approvals.VerifyJson(JsonConverter.ToJson(descriptionResult));
    }
}
