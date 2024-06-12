using ApiValidations.ApiValidators;
using ApprovalTests;
using FluentAssertions;
using PopValidations;
using PopValidations_Tests.TestHelpers;
using System.Xml;

namespace ApiValidations_Tests;

public record ModerateInputObject(int value);
public record ModerateOutputObject(int value);
public enum ModerateEnum
{
    TestValue1,
    TestValue2
}

public class ModerateTestApi
{
    public string IsCustom(string value) { return string.Empty; }
    public string IsEmail(string email) { return string.Empty; }
    public string IsEmpty(string empty) { return string.Empty; }
    public string IsEnum(string enumValue){ return string.Empty; }
    public string IsEqualTo(string value) { return string.Empty; }
    public int IsGreaterThan(int greaterThan) { return 0; }
    public int IsGreaterThanOrEqualTo(int greaterThanOrEqualTo) { return 0; }
    public int IsLengthExclusivelyBetween(int lengthBetween) { return 0; }
    public int IsLengthInclusivelyBetween(int lengthBetweenInclusive) { return 0; }
    public int IsLessThan(int lessThan) { return 0; }
    public int IsLessThanOrEqual(int lessThan) { return 0; }
    public string IsNotEmpty(string empty) { return string.Empty; }
    public string IsNotNull(string? notNull) { return string.Empty; }
    public int Switch(int value) { return 0; }

    public IEnumerable<int> EnumerableInt(IEnumerable<int> values) { return values; }

    public ModerateOutputObject Reusable1(ModerateInputObject value) { return null; }
    public ModerateOutputObject Reusable2(ModerateInputObject value) { return null; }
}

public class ModerateTestApiValidator : ApiValidator<ModerateTestApi>
{
    public ModerateTestApiValidator()
    {
        //DescribeFunc(x => x.IsCustom(Param.Is<string>().IsCustom("Checks for things", c => c == string.Empty))).Returns.IsCustom(x => x == string.Empty);
        DescribeFunc(x => x.IsEmail(Param.Is<string>().IsEmail())).Return.IsEmail();
        DescribeFunc(x => x.IsEmpty(Param.Is<string>().IsEmpty())).Return.IsEmpty();
        DescribeFunc(x => x.IsEnum(Param.Is<string>().IsEnum(typeof(ModerateEnum)))).Return.IsEnum(typeof(ModerateEnum));
        DescribeFunc(x => x.IsEqualTo(Param.Is<string>().IsEqualTo(string.Empty))).Return.IsEqualTo(string.Empty);
        DescribeFunc(x => x.IsGreaterThan(Param.Is<int>().IsGreaterThan(10))).Return.IsGreaterThan(10);
        DescribeFunc(x => x.IsGreaterThanOrEqualTo(Param.Is<int>().IsGreaterThanOrEqualTo(10))).Return.IsGreaterThanOrEqualTo(10);
        DescribeFunc(x => x.IsLessThan(Param.Is<int>().IsLessThan(10))).Return.IsLessThan(10);
        DescribeFunc(x => x.IsLessThanOrEqual(Param.Is<int>().IsLessThanOrEqualTo(10))).Return.IsLessThanOrEqualTo(10);
        DescribeFunc(x => x.IsLengthExclusivelyBetween(Param.Is<int>().IsLengthExclusivelyBetween(1,10))).Return.IsLengthExclusivelyBetween(0,10);
        DescribeFunc(x => x.IsLengthInclusivelyBetween(Param.Is<int>().IsLengthInclusivelyBetween(1, 10))).Return.IsLengthInclusivelyBetween(0, 10);
        DescribeFunc(x => x.IsNotEmpty(Param.Is<string>().IsNotEmpty())).Return.IsNotEmpty();
        DescribeFunc(x => x.IsNotNull(Param.Is<string?>().IsNotNull())).Return.IsNotNull();
        //DescribeFunc(x => x.Switch(Param.Is<int>().Switch(x => x.Case(5, "5", "dont be 5")))).Return.Switch(x => x.Case(5, "5", "dont be 5"));
        //DescribeFuncEnumerable(x => x.EnumerableInt(Param.IsEnumerable<int>().ForEach(x => x.IsNotNull())).Return.ForEach(x => x.IsNotNull());
    }
}

public class ModerateTest
{
    [Fact]
    public async Task GivenAValidator_WithErrors_ThenEveryFieldHasErrors()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new ModerateTestApiValidator());

        // Act
        var validationResult = await runner.Validate(new ModerateTestApi());
        var json = JsonConverter.ToJson(validationResult);

        // Assert
        validationResult.Errors.Should().HaveCount(0);
        Approvals.VerifyJson(json);
    }

    [Fact]
    public void GivenAValidator_WhenDescribing_ThenEveryFieldHasDescriptions()
    {
        // Arrange
        var runner = ValidationRunnerHelper.BasicRunnerSetup(new ModerateTestApiValidator());

        // Act
        var description = runner.Describe();
        var json = JsonConverter.ToJson(description);

        // Assert
        description.Results.Should().HaveCount(9);
        Approvals.VerifyJson(json);
    }
}