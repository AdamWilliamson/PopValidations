using ApiValidations;
using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Functional_Testbed;
using PopValidations_Tests.TestHelpers;
using System.Collections;

namespace ApiValidations_Tests;

public record InputObject(int value);
public record OutputObject(int value);

public class TestApi
{
    public void InputFunction1(int value) { }
    public void InputFunction1(int value, string anothervalue) { }
    public int InputFunction2(InputObject value) { return 1; }
    public List<int> EnumerableReturnFunc() { return []; }

    public SubApi ChildObject { get; set; } = new();
}

public class SubApi 
{
    public void InputFunction1(int value) { }
}

public class TestApiValidator : ApiValidator<TestApi>
{
    public TestApiValidator()
    {         
        var stringParam = Param.Is<string>().IsNotNull().IsEmail();

        DescribeFunc(x => x.InputFunction1(Param.Is<int>().IsNotNull().IsGreaterThan(10))).Return.IsNotNull();
        DescribeFunc(x => x.InputFunction1(Param.Is<int>().IsNotNull().IsGreaterThan(0), stringParam)).Return.IsNotNull();
        DescribeFunc(x => x.InputFunction2(Param.Is<InputObject>().SetValidator(new InputObjectValidator()))).Return.IsNotNull();

        DescribeFuncEnumerable(x => x.EnumerableReturnFunc());

        Describe(x => x.ChildObject).SetValidator(new SubApiValidator());
    }
}

public class SubApiValidator : ApiSubValidator<SubApi>
{
    public SubApiValidator()
    {
        DescribeFunc(x => x.InputFunction1(Param.Is<int>().IsNotNull().IsGreaterThan(100))).Return.IsNotNull();//.Return.IsNotNull()
    }
}

public class InputObjectValidator : AbstractSubValidator<InputObject>
{
    public InputObjectValidator()
    {
        Describe(x => x.value).IsNotNull();
    }
}

public class UnitTest1
{
    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new TestApiValidator());

        // Act
        var validationResult = await runner.Validate(new TestApi());
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(0);
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new TestApiValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(9);
        Approvals.VerifyJson(json);
    }
}