using ApprovalTests;
using ApiValidations_Tests.TestHelpers;
using ApiValidations.Execution;
using FluentAssertions;

namespace ApiValidations_Tests.ReturnTests.ScopeValidationTests;

public class Scope_DemoTest
{
    [Fact]
    public void Description()
    {
        // Arrange
        var descriptionRunner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

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
        yield return new ObjectFunctionValidationTestDescription<Level3>(
            "Child[0].Child[0]",
            nameof(Level3.Check),
            0,
            [],
            $"Is not equal to 'True'."
        );
        yield return new ObjectFunctionValidationTestDescription<Level4>(
            "Child[0].Child[0].Child[0]",
            nameof(Level4.Check),
            0,
            [],
            $"Is not equal to 'True'."
        );
        yield return new ObjectFunctionValidationTestDescription<Level5>(
            "Child[0].Child[0].Child[0].Child[0]",
            nameof(Level5.Check),
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

        // Act
        var results = await runner.ValidateAndExecute(
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
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new Level1Validator());

        // Act
        var results = await runner.ValidateAndExecute(
            ModelCreation.GenerateTestData(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(Level1).GetMethod(nameof(Level1.Check))!,
                []
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}
