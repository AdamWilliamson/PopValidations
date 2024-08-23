using ApiValidations_Tests.GenericTestableObjects;
using System.Linq.Expressions;
using ApiValidations;
using ApiValidations_Tests.TestHelpers;
using PopValidations.Validations;

namespace ApiValidations_Tests.ParamTests.IsCustomValidationTests;

public static partial class ApiTestConstructor
{
    public static object[] BuildIsTest(Expression<Action<TestValidator<BasicDataTypes>>> expression, int paramIndex)
    {
        return TestConstructor.BuildParamTest(
            (v) => v.DescribeFunc(dt => dt.NoReturnIntParam(v.Param.Is<int>().IsNotNull())),
            (paramIndex, nameof(IsCustomValidation<int>), "Must be min value.", null)
        );
    }
}
