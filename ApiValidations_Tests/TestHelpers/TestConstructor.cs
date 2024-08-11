using PopValidations_Tests.TestHelpers;
using ApiValidations_Tests.GenericTestableObjects;
using System.Reflection;
using System.Linq.Expressions;
using ApiValidations;

namespace ApiValidations_Tests.TestHelpers;

public class TestValidator<T> : ApiValidator<T>
{
    public TestValidator(Expression<Action<TestValidator<T>>> validations)
    {
        validations?.Compile().Invoke(this);
    }
}

public static partial class TestConstructor
{
    public static object[] BuildParam(
           Expression<Action<TestValidator<BasicDataTypes>>> expression,
           params Action<DescriptionResultAssertions, MethodInfo>[] resultAssertions
       )
    {
        var method = (((((MethodCallExpression)expression.Body).Arguments[0] as UnaryExpression).Operand as LambdaExpression).Body as MethodCallExpression).Method;

        return [
            expression,
            resultAssertions
                .Select(i => (Action<DescriptionResultAssertions>)((results) => i.Invoke(results, method)))
                .ToArray()
        ];
    }

    public static object[] BuildParamTest(
        Expression<Action<TestValidator<BasicDataTypes>>> expression,
        (int ParamIndex, string ValidationName, string Description, (string Key, string Value)[]? KeyValuePairs) ValidationTest
    )
    {
        return BuildParam(
            expression,
            (results, mi) => results.ContainsParam(
                mi,
                ValidationTest.ParamIndex,
                ValidationTest.ValidationName,
                ValidationTest.Description,
                ValidationTest.KeyValuePairs
            )
        );
    }
}