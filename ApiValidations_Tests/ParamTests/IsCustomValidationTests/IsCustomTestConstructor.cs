using ApiValidations_Tests.GenericTestableObjects;
using PopValidations.Validations;
using System.Linq.Expressions;
using ApiValidations;

namespace ApiValidations_Tests.TestHelpers;

public static partial class TestConstructor
{
    public static object[] BuildIsTest(Expression<Action<TestValidator<BasicDataTypes>>> expression, int paramIndex)
    {
        return TestConstructor.BuildParamTest(
            (v) => v.DescribeFunc(dt => dt.NoReturnIntParam(v.Param.Is<int>().IsNotNull())),
            (paramIndex, nameof(IsCustomValidation<int>), "Must be min value.", null)
        );
    }
}
