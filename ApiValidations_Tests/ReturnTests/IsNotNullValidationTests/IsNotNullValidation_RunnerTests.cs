﻿
using ApprovalTests;
using FluentAssertions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;

namespace ApiValidations_Tests.ReturnTests.IsNotNullValidationTests;

public class IsNotNullApi
{
    public string? Get1() { return null; }
    public long? Get2() { return 1; }
}

public class IsNotNull_TestingValidator : ApiValidator<IsNotNullApi>
{
    public IsNotNull_TestingValidator()
    {
        DescribeFunc(x => x.Get1()).Return.IsNotNull();

        DescribeFunc(x => x.Get2()).Return.IsNotNull();
    }
}

public class IsNotNullValidation_RunnerTests
{
    [Fact]
    public void WhenDescribing_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotNull_TestingValidator());

        // Act
        var description = runner.Describe();

        // Assert
        description.Results.Should().HaveCount(2);
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<IsNotNullApi>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(JsonConverter.ToJson(description));
    }

    [Fact]
    public async Task WhenValidating_ItReturnsTheValidation()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotNull_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsNotNullApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsNotNullApi).GetMethod(nameof(IsNotNullApi.Get1))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(1);
        Approvals.VerifyJson(JsonConverter.ToJson(validation));
    }

    [Fact]
    public async Task WhenValidating_ItIsSuccessful()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new IsNotNull_TestingValidator());

        // Act
        var validation = await runner.Validate(
            new IsNotNullApi(),
            new ApiValidations.Execution.HeirarchyMethodInfo(
                string.Empty,
                typeof(IsNotNullApi).GetMethod(nameof(IsNotNullApi.Get2))!,
                []
            )
        );

        // Assert
        validation.Errors.Should().HaveCount(0);
    }
}
