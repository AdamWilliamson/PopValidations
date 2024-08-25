using ApprovalTests;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;
using FluentAssertions;
using PopValidations;

namespace ApiValidations_Tests.ReturnTests.ScopeValidationTests.Scope_IncludeInsideForEachWhenValidationTests;

public class Validator : ApiValidator<Level1>
{
    public Validator()
    {
        Scope(
           "Database Value",
           (x) => DataRetriever.GetValue(x),
           (retrievedData) =>
           {
               DescribeFunc(x => x.Check()).Return.IsEqualTo(retrievedData);

               DescribeEnumerable(x => x.Child).ForEach(c => c.SetValidator(new SubValidator()));
           }
        );
    }
}

public class SubValidator : ApiSubValidator<Level2>
{
    public SubValidator()
    {
        Include(new SubValidatorToInclude());
    }
}

public class SubValidatorToInclude : ApiSubValidator<Level2>
{
    public SubValidatorToInclude()
    {
        DescribeFunc(x => x.Check()).Return.IsEqualTo(true);

        Scope(
            "Another DB Value",
            (x) => DataRetriever.GetMoreValue(x),
            (moreData) =>
            {
                DescribeFunc(x => x.Check()).Return.IsEqualTo(moreData);
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

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new ObjectFunctionValidationTestDescription<Level2>(
            "Child[0]",
            nameof(Level2.Check),
            0,
            [],
            $"Is not equal to 'True'."
        );
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(IObjectFunctionValidationTestDescription description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Validator());

        // Act
        var results = await runner.Validate(
            ModelCreation.GenerateInvalidTestData(),
            new HeirarchyMethodInfo(
                description.ObjectMap,
                description.ApiType.GetMethod(description.Function)!,
                description.ParamInputs.ToList()
            )
        );

        // Assert
        var methodInfo = description.ApiType.GetMethod(description.Function)!;
        results.Errors.Should().HaveCount(1);
        results.Should().ContainsReturn(
            description.ObjectMap,
            methodInfo,
            description.Error
        );
    }

    [Fact]
    public async Task WhenValidating_ItSucceeds()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Validator());

        // Act
        var results = await runner.Validate(
            ModelCreation.GenerateTestData(),
            new HeirarchyMethodInfo(
                "Child[0]",
                typeof(Level2).GetMethod(nameof(Level2.Check))!,
                []
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
