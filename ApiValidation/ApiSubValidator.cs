using System.Linq.Expressions;

namespace PopValidations_Functional_Testbed;

public abstract class ApiSubValidator<TValidationType> : PopValidations.AbstractSubValidator<TValidationType>, IValidator
{
    protected ParamValidationSetBuilder<TValidationType> Builder;
    protected ParamBuilder<TValidationType> Param;

    protected ApiSubValidator()
    {
        Builder = new(this);
        Param = new(new ParamVisitor<TValidationType>(this, Builder));
    }

    ParamDetailsDTO? IValidator.GetCurrentParamDescriptor() => Builder.CurrentParam;

    //public IFunctionDescriptor DescribeParams(Expression<Action<TValidationType>> expression)
    //{
    //    var method = ((MethodCallExpression)expression.Body);

    //    var args2 = (from arg in method.Arguments
    //                 select Expression.Lambda(arg, null)
    //                ).ToList();

    //    var actualParams = method.Method.GetParameters();
    //    if (actualParams.Length != args2.Count()) throw new Exception("Parameter Exception Failed.");

    //    Builder.SetCurrentFunction(method.Method);

    //    for (var i = 0; i < actualParams.Length; i++)
    //    {
    //        var param = args2[i];
    //        var paramDetails = actualParams[i];
    //        Builder.SetCurrentParam(paramDetails.Name, paramDetails.ParameterType);
    //        param.Compile().DynamicInvoke();
    //    }

    //    return (IFunctionDescriptor)Builder.CurrentFunction;

    //    //return new ReturnDescriptor<TValidationType>();
    //}


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
        var funcDescriptor = Builder.SetCurrentFunction<TFuncOutput>(method.Method);
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
