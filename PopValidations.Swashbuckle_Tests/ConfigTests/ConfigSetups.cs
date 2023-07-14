using PopValidations.Swashbuckle_Tests.ConfigTests.Setups;
using PopValidations.Swashbuckle_Tests.Helpers;
using System.Collections;

namespace PopValidations.Swashbuckle_Tests.ConfigTests;

public class ConfigSetups : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new TestSetup<
                NotNullTestSetup.TestController,
                NotNullTestSetup.RequestValidator,
                NotNullTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsNullTestSetup.TestController,
                IsNullTestSetup.RequestValidator,
                IsNullTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                NotNullTestSetup.TestController,
                NotNullTestSetup.RequestValidator,
                NotNullTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsEmptyTestSetup.TestController,
                IsEmptyTestSetup.RequestValidator,
                IsEmptyTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsNotEmptyTestSetup.TestController,
                IsNotEmptyTestSetup.RequestValidator,
                IsNotEmptyTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsGreaterThanTestSetup.TestController,
                IsGreaterThanTestSetup.RequestValidator,
                IsGreaterThanTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsGreaterThanOrEqualToTestSetup.TestController,
                IsGreaterThanOrEqualToTestSetup.RequestValidator,
                IsGreaterThanOrEqualToTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsLessThanTestSetup.TestController,
                IsLessThanTestSetup.RequestValidator,
                IsLessThanTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsLessThanOrEqualToTestSetup.TestController,
                IsLessThanOrEqualToTestSetup.RequestValidator,
                IsLessThanOrEqualToTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsLengthInclusivelyBetweenTestSetup.TestController,
                IsLengthInclusivelyBetweenTestSetup.RequestValidator,
                IsLengthInclusivelyBetweenTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsLengthExclusivelyBetweenTestSetup.TestController,
                IsLengthExclusivelyBetweenTestSetup.RequestValidator,
                IsLengthExclusivelyBetweenTestSetup.Request
            >()
        };
        yield return new object[]
        {
            new TestSetup<
                IsEnumTestSetup.TestController,
                IsEnumTestSetup.RequestValidator,
                IsEnumTestSetup.Request
            >()
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
