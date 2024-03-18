using System.Linq.Expressions;

namespace PopValidations_Functional_Testbed;

public interface IValidator
{
    ParamDetailsDTO? GetCurrentParamDescriptor();
}

public abstract class ApiValidator<TValidationType> : PopValidations.AbstractValidator<TValidationType>,  IValidator
{
    protected ParamValidationSetBuilder<TValidationType> Builder;
    protected ParamBuilder<TValidationType> Param;

    protected ApiValidator()
    {
        Builder = new(this);
        Param = new(new ParamVisitor<TValidationType>(this, Builder));
    }

    ParamDetailsDTO? IValidator.GetCurrentParamDescriptor() => Builder.CurrentParam;

    public IFunctionDescriptor<IEnumerable<TFuncOutput>> DescribeFuncEnumerable<TFuncOutput>(Expression<Func<TValidationType, IEnumerable<TFuncOutput>>> expression)
    {
        var method = ((MethodCallExpression)expression.Body);
        var funcDescriptor = Builder.SetCurrentFunction<IEnumerable<TFuncOutput>>(method.Method);
        DescribeFuncImpl(method);

        return funcDescriptor;
    }

    public IFunctionDescriptor<TFuncOutput> DescribeFunc<TFuncOutput>(Expression<Func<TValidationType, TFuncOutput>> expression)
    {
        var method = ((MethodCallExpression)expression.Body);
        var funcDescriptor= Builder.SetCurrentFunction<TFuncOutput>(method.Method);
        DescribeFuncImpl(method);

        return funcDescriptor;
    }

    public IFunctionDescriptor DescribeFunc(Expression<Action<TValidationType>> expression)
    {
        var method = ((MethodCallExpression)expression.Body);
        var funcDescriptor = Builder.SetCurrentFunction(method.Method);
        DescribeFuncImpl(method);

        return funcDescriptor;
    }

    protected void DescribeFuncImpl(MethodCallExpression method)
    {
        var args2 = (from arg in method.Arguments
                     select Expression.Lambda(arg, null)
                    ).ToList();

        var actualParams = method.Method.GetParameters();
        if (actualParams.Length != args2.Count()) throw new Exception("Parameter Exception Failed.");

        for (var i = 0; i < actualParams.Length; i++)
        {
            var param = args2[i];
            var paramDetails = actualParams[i];
            Builder.SetCurrentParam(paramDetails.Name, paramDetails.ParameterType);
            param.Compile().DynamicInvoke();
        }
    }
}