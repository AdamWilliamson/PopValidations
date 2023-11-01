using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using PopValidations_Tests.ValidationsTests.GenericTestableObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PopValidations_Tests.ValidationsTests.SetValidatorTests;

public record V1BaseClass(V1Depth2Class Depth2);
public record V1Depth2Class(int TestInt);



public class V1BaseClass_TestingValidator : AbstractValidator<V1BaseClass>
{
    public V1BaseClass_TestingValidator()
    {
        Describe(x => x.Depth2)
            .Vitally().IsNotNull()
            .SetValidator(new V1Depth2Class_TestingValidator());
    }
}

public class V1Depth2Class_TestingValidator : AbstractSubValidator<V1Depth2Class>
{
    public V1Depth2Class_TestingValidator()
    {
        Describe(x => x.TestInt).IsGreaterThan(5);
    }
}

public record V2BaseClass(List<V2Depth2Class> Depth2List);
public record V2Depth2Class(List<V2Depth3Class> Depth3List);
public record V2Depth3Class(int TestInt);

public class V2BaseClass_TestingValidator : AbstractValidator<V2BaseClass>
{
    public V2BaseClass_TestingValidator()
    {
        DescribeEnumerable(x => x.Depth2List)
            .ForEach(x => x.SetValidator(new V2Depth2Class_TestingValidator()));
    }
}

public class V2Depth2Class_TestingValidator : AbstractSubValidator<V2Depth2Class>
{
    public V2Depth2Class_TestingValidator()
    {
        DescribeEnumerable(x => x.Depth3List)
            .ForEach(x => x.SetValidator(new V2Depth3Class_TestingValidator()));
    }
}

public class V2Depth3Class_TestingValidator : AbstractSubValidator<V2Depth3Class>
{
    public V2Depth3Class_TestingValidator()
    {
        Describe(x => x.TestInt).IsGreaterThan(5);
    }
}



public class SetValidator_RunnerTests
{
    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProducedV1()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new V1BaseClass_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new V1BaseClass(Depth2: new V1Depth2Class(10)));

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrorsV1()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new V1BaseClass_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new V1BaseClass(Depth2: new V1Depth2Class(1)));
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(1);
        Approvals.VerifyJson(json);
    }


    [Fact]
    public async Task GivenAValidator_WithNoErrors_ThenNoErrorsAreProducedV2()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new V2BaseClass_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new V2BaseClass(Depth2List: new() { new V2Depth2Class(new() { new V2Depth3Class(10) }), new V2Depth2Class(new() { new V2Depth3Class(10) }) }));

        // Assert
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrorsV2()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new V2BaseClass_TestingValidator());

        // Act
        var validationResult = await runner.Validate(new V2BaseClass(Depth2List: new() { new V2Depth2Class(new() { new V2Depth3Class(1) }), new V2Depth2Class(new() { new V2Depth3Class(1) }) }));
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(2);
        Approvals.VerifyJson(json);
    }
}
