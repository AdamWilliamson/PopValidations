using ApiValidations;
using ApprovalTests;
using PopValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ParamTests.ForEachTests;

public class FunctionsWithReturnTypesValidator : ApiValidator<FunctionsWithReturnTypes>
{
    public FunctionsWithReturnTypesValidator()
    {
        DescribeFunc(x => x.EnumerableStringReturn())
            .Return.ForEach(x => x.IsNotNull());
    }
}

public class ForEachBasicTests
{
    [Fact]
    public void Thing()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new FunctionsWithReturnTypesValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        Approvals.VerifyJson(json);
    }
}
