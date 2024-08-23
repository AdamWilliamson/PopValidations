using System.Linq.Expressions;
using ApiValidations.Descriptors;
using ApiValidations.Descriptors.Core;
using PopValidations.ValidatorInternals;

namespace ApiValidations;

public interface IApiSubValidator<TValidationType> { }

public abstract class ApiSubValidator<TValidationType> : PopValidations.AbstractSubValidator<TValidationType>, IValidator, IApiSubValidator<TValidationType>
{
    protected ParamValidationSetBuilder<TValidationType> Builder;
    protected ParamBuilder<TValidationType> Param;

    protected ApiSubValidator()
    {
        Builder = new(this);
        Param = new(new ParamVisitor<TValidationType>(this, Builder));
    }

    ParamDetailsDTO? IValidator.GetCurrentParamDescriptor() => Builder.CurrentParam;

    public IFunctionDescriptor<IEnumerable<TFuncOutput>> DescribeFuncEnumerable<TFuncOutput>(Expression<Func<TValidationType, IEnumerable<TFuncOutput>>> expression)
    {
        var method = (MethodCallExpression)expression.Body;
        var funcDescriptor = Builder.SetCurrentFunction<IEnumerable<TFuncOutput>>(method.Method);
        DescribeFuncImpl(method);

        return new FunctionDescriptor<TValidationType, IEnumerable<TFuncOutput>>(funcDescriptor, ((IStoreContainer)this).Store);
    }

    public IFunctionDescriptor<TFuncOutput> DescribeFunc<TFuncOutput>(Expression<Func<TValidationType, TFuncOutput>> expression)
    {
        var method = (MethodCallExpression)expression.Body;
        var funcDescriptor = Builder.SetCurrentFunction<TFuncOutput>(method.Method);
        DescribeFuncImpl(method);

        return new FunctionDescriptor<TValidationType, TFuncOutput>(funcDescriptor, ((IStoreContainer)this).Store);
    }

    public IFunctionDescriptor DescribeFunc(Expression<Action<TValidationType>> expression)
    {
        var method = (MethodCallExpression)expression.Body;
        var funcDescriptor = Builder.SetCurrentFunction(method.Method);
        DescribeFuncImpl(method);

        return new FunctionDescriptor<TValidationType>(funcDescriptor, ((IStoreContainer)this).Store);
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
