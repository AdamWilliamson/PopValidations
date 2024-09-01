using ApiValidations;
using ApiValidations.Execution;
using ApiValidations_Tests.TestHelpers;
using ApprovalTests;
using ApprovalTests.Namers;
using FluentAssertions;
using PopValidations;

namespace ApiValidations_Tests.ParamTests.IsEmailValidationTests;

public class EmailApi
{
    public void GetEmail(string email) { }
    public void GetEmail_Custom(string email) { }
}

public class IsEmail_TestingValidator : ApiValidator<EmailApi>
{
    public IsEmail_TestingValidator()
    {
        DescribeFunc(x => x.GetEmail(Param.Is<string>().IsEmail()));
        DescribeFunc(x => x.GetEmail_Custom(Param.Is<string>().Vitally().IsEmail(o => o.WithDescription("Don't make a mistake.").WithErrorMessage("Mistake"))));
    }
}

public partial class IsEmailValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmail_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<EmailApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    public static IEnumerable<object[]> ErroringValues()
    {
        yield return new FunctionValidationTestDescription<EmailApi>(nameof(EmailApi.GetEmail), 0, ["Not An Email"], "Is not a valid email.");
        yield return new FunctionValidationTestDescription<EmailApi>(nameof(EmailApi.GetEmail_Custom),0, ["Not An Email"], "Mistake");
    }

    [Theory]
    [MemberData(nameof(ErroringValues))]
    public async Task WhenValidating_ItReturnsTheValidation(FunctionValidationTestDescription<EmailApi> description)
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmail_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new EmailApi(), 
            new HeirarchyMethodInfo(
                string.Empty,
                description.ApiType.GetMethod(description.Function)!,
                description.ParamInputs.ToList()
            )
        );

        // Assert
        results.Errors.Should().HaveCount(1);
        results.Should().ContainsParam(
            description.ApiType.GetMethod(description.Function)!,
            description.ParamIndex,
            description.Error
        );

        using (ApprovalResults.ForScenario($"{description.Function}_Param({description.ParamIndex})"))
        {
            Approvals.VerifyJson(JsonConverter.ToJson(results));
        }
    }


    [Fact]
    public async Task WhenValidating_ItSucceeds()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsEmail_TestingValidator());

        // Act
        var results = await runner.ValidateAndExecute(
            new EmailApi(),
            new HeirarchyMethodInfo(
                string.Empty,
                typeof(EmailApi).GetMethod(nameof(EmailApi.GetEmail))!,
                ["Api@Validations.com"]
            )
        );

        // Assert
        results.Errors.Should().HaveCount(0);
    }
}