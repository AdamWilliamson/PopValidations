using ApprovalTests;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;
using FluentAssertions;

namespace ApiValidations_Tests.ParamTests.ScopeValidationTests.Scope_IncludeValidationTests;

public class Validator : ApiValidator<Level1>
{
    public Validator()
    {
        Include(new SubValidator());
    }
}

public class SubValidator : ApiSubValidator<Level1>
{
    public SubValidator()
    {
        DescribeFunc(x => x.Check(Param.Is<bool>().IsEqualTo(true)));

        Scope(
            "Database Value",
            (x) => DataRetriever.GetValue(x),
            (retrievedData) =>
            {
                DescribeFunc(x => x.Check(Param.Is<bool>().IsEqualTo(retrievedData)));

                Scope(
                    "Another DB Value",
                    (x) => DataRetriever.GetMoreValue(x),
                    (moreData) =>
                    {
                        DescribeFunc(x => x.Check(Param.Is<bool>().IsEqualTo(moreData)));
                    }
                );
            }
        );
    }
}

public class Scope_IncludeTest
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

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new ObjectFunctionValidationTestDescription<Level1>(
            "",
            nameof(Level1.Check),
            0,
            [false],
            $"Is not equal to 'True'."
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(ObjectFunctionValidationTestDescription<Level1> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Validator());

        // Act
        var results = await runner.ValidateAndExecute(
            ModelCreation.GenerateTestData(),
            new HeirarchyMethodInfo(
                description.ObjectMap,
                description.ApiType.GetMethod(description.Function)!,
                description.ParamInputs.ToList()
            )
        );

        // Assert
        var methodInfo = description.ApiType.GetMethod(description.Function)!;
        results.Errors.Should().HaveCount(methodInfo.GetParameters().Count());
        results.Should().ContainsParam(
            methodInfo,
            description.ParamIndex,
            description.Error
        );
    }

    [Fact]
    public async Task WhenValidating_ItSucceeds()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Validator());

        // Act
        var results = await runner.ValidateAndExecute(
            ModelCreation.GenerateTestData(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(Level1).GetMethod(nameof(Level1.Check))!,
                [true]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
