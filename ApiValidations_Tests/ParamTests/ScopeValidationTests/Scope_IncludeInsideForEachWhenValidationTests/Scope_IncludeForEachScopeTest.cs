using ApprovalTests;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace ApiValidations_Tests.ParamTests.ScopeValidationTests.Scope_IncludeInsideForEachWhenValidationTests;

//public static class DataRetriever
//{
//    public static Task<string> GetValue(Base v)
//    {
//        return Task.FromResult(v?.DependantField + " GetValue");
//    }

//    public static Task<string> GetMoreValue(Base v)
//    {
//        return Task.FromResult(v?.DependantField + " GetMoreValue");
//    }
//}

//public record Base(string? DependantField);
//public record Level1(bool Check, string? DependantField, Level2? Child) : Base(DependantField);
//public record Level2(bool Check, string? DependantField, Level3? Child) : Base(DependantField);
//public record Level3(bool Check, string? DependantField, Level4? Child) : Base(DependantField);
//public record Level4(bool Check, string? DependantField, Level5? Child) : Base(DependantField);
//public record Level5(bool Check, string? DependantField);

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

public class Scope_IncludeForEachScopeTest
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
