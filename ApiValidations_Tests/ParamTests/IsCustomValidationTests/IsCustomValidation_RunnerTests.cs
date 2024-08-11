using ApprovalTests;
using FluentAssertions;
using PopValidations;
using ApiValidations_Tests.GenericTestableObjects;
using ApiValidations;
using PopValidations.Validations;
using FluentAssertions.Execution;
using ApiValidations_Tests.TestHelpers;

namespace PopValidations_Tests.ValidationsTests.IsCustomValidationTests;

public class IsCustom_TestingValidator : ApiValidator<BasicDataTypes>
{
    public IsCustom_TestingValidator()
    {
        var intParam1Validation = Param.Is<int>().Is("Must be min value.", "Is not min value.", x => x == int.MinValue);
        var decimalParam1Validation = Param.Is<decimal>().Is("Must be min value.", "Is not min value.", x => x == decimal.MinValue);
        var stringParam1Valdiation = Param.Is<string>().Is("Must be aaaaa.", "Is not aaaaa.", x => x == "aaaaa");

        DescribeFunc(x => x.IntReturnNoParams()).Return.Is("Must be min value.", "Is not min value.", x => x == int.MinValue);
        DescribeFunc(x => x.DecimalReturnNoParams()).Return.Is("Must be min value.", "Is not min value.", x => x == decimal.MinValue);
        DescribeFunc(x => x.StringReturnNoParams()).Return.Is("Must be aaaaa.", "Is not aaaaa.", x => x == "aaaaa");

        DescribeFunc(x => x.NoReturnIntParam(intParam1Validation));
        DescribeFunc(x => x.NoReturnDecimalParam(decimalParam1Validation));
        DescribeFunc(x => x.NoReturnStringParam(stringParam1Valdiation));

        DescribeFunc(x => x.IntReturnIntDecimalParam(intParam1Validation, decimalParam1Validation))
            .Return.Is("Must be min value.", "Is not min value.", x => x == int.MinValue);

        DescribeFunc(x => x.DecimalReturnDecimalStringParam(decimalParam1Validation, stringParam1Valdiation))
            .Return.Is("Must be min value.", "Is not min value.", x => x == decimal.MinValue);

        DescribeFunc(x => x.StringReturnStringIntParam(stringParam1Valdiation, intParam1Validation))
            .Return.Is("Must be aaaaa.", "Is not aaaaa.", x => x == "aaaaa");
    }
}

public class IsCustomValidation_RunnerTests
{
    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(
            validator: new IsCustom_TestingValidator()
        );

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        using var _ = new AssertionScope();

        description.Should().ContainsParam(
            (BasicDataTypes x) => x.NoReturnIntParam(FakeParam.Is<int>()),
            0,
            nameof(IsCustomValidation<int>),
            "Must be min value.",
            null
        );
        description.Results.Should().HaveCount(ValidatableHelper.GetValidatableCount<BasicDataTypes>(ValidatableType.NoExceptions));
        Approvals.VerifyJson(json);
    }
}
