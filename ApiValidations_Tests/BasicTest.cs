using ApiValidations.ApiValidators;
using ApprovalTests;
using FluentAssertions;
using FluentAssertions.Execution;
using PopValidations;
using PopValidations_Tests.TestHelpers;

namespace ApiValidations_Tests;

public record BasicInputObject(int value);
public record BasicOutputObject(int value);

public class BasicTestApi
{
    public void InputFunction1(int value) { }
    public void InputFunction1(int value, string anothervalue) { }
    public int InputFunction2(BasicInputObject value) { return 1; }
    public List<int> EnumerableReturnFunc() { return []; }
    public void EnumerableParamFunc(List<int> enumerableParam) {}

    public BasicSubApi ChildObject { get; set; } = new();
}

public class BasicSubApi 
{
    public void InputFunction1(int value) { }
    public string InputFunction2(int value) { return ""; }
}

public class BasicTestApiValidator : ApiValidator<BasicTestApi>
{
    public BasicTestApiValidator()
    {
        var stringParam = Param.Is<string>().IsNotNull().IsEmail();

        DescribeFunc(x => x.InputFunction1(Param.Is<int>().IsNotNull().IsGreaterThan(10)));
        DescribeFunc(x => x.InputFunction1(Param.Is<int>().IsNotNull().IsGreaterThan(0), stringParam));
        DescribeFunc(x => x.InputFunction2(Param.Is<BasicInputObject>().SetValidator(new BasicInputObjectValidator()))).TypedReturn.IsNotNull();

        Describe(x => x.ChildObject).SetValidator(new BasicSubApiValidator());

        DescribeFunc(x =>
            x.EnumerableParamFunc(
                Param.IsEnumerable<int>().ForEach(x => x.IsNotNull())
                .Convert<List<int>>()
            )
        );
    }
}

public class BasicSubApiValidator : ApiSubValidator<BasicSubApi>
{
    public BasicSubApiValidator()
    {
        DescribeFunc(x => x.InputFunction1(Param.Is<int>().IsNotNull().IsGreaterThan(100)));
        DescribeFunc(x => x.InputFunction2(Param.Is<int>().IsNotNull().IsGreaterThan(100))).TypedReturn.IsNotNull();
    }
}

public class BasicInputObjectValidator : AbstractSubValidator<BasicInputObject>
{
    public BasicInputObjectValidator()
    {
        Describe(x => x.value).IsNotNull();
    }
}

public class BasicTest
{
    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicTestApiValidator());

        // Act
        var validationResult = await runner.Validate(new BasicTestApi());
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(0);
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new BasicTestApiValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        AssertionOptions.FormattingOptions.MaxLines = 500;
        description.Results.Should().HaveCount(9);
        Approvals.VerifyJson(json);
    }
}