using PopValidations.ValidatorInternals;
using System.Reflection;

namespace ApiValidations.Descriptors.Core;

public class ParamValidationSetBuilder<TValidationType>
{
    private List<IFunctionExpressionToken> Functions = new();
    public IFunctionExpressionToken? CurrentFunction = null;
    public ParamDetailsDTO? CurrentParam = null;
    private readonly IStoreContainer owner;

    public ParamValidationSetBuilder(IStoreContainer owner)
    {
        this.owner = owner;
    }

    public IFunctionExpressionToken SetCurrentFunction<TReturnType>(MethodInfo methodInfo)
    {
        var paramsList = methodInfo.GetParameters().Select(x => new ParamDetailsDTO(x.Name, x.ParameterType, x.Position)).ToList();
        if (SetCurrentFunction(
            methodInfo.Name,
            methodInfo.ReturnType,
            paramsList
        ))
        {
            return CurrentFunction as IFunctionExpressionToken ?? throw new Exception("Invalid Function Type");
        }

        var func = //new FunctionDescriptor<TValidationType, TReturnType>(
            new FunctionExpressionToken<TValidationType>(methodInfo.Name, methodInfo.ReturnType, paramsList);
        //    , owner.Store
        //);
        CurrentFunction = func;
        Functions.Add(CurrentFunction);
        return func;
    }

    public IFunctionExpressionToken SetCurrentFunction(MethodInfo methodInfo)
    {
        var paramsList = methodInfo.GetParameters().Select(x => new ParamDetailsDTO(x.Name, x.ParameterType, x.Position)).ToList();
        if (SetCurrentFunction(methodInfo.Name, methodInfo.ReturnType, paramsList))
        {
            return CurrentFunction as IFunctionExpressionToken ?? throw new Exception("Invalid Function Type");
        }

        var func = //new FunctionDescriptor<TValidationType>(
            new FunctionExpressionToken<TValidationType>(methodInfo.Name, methodInfo.ReturnType, paramsList);
            //owner.Store);
        Functions.Add(func);
        CurrentFunction = func;
        return CurrentFunction as IFunctionExpressionToken ?? throw new Exception("Invalid Function Type"); ;
    }

    public bool SetCurrentFunction(string name, Type returnType, List<ParamDetailsDTO> paramList)
    {
        CurrentFunction = Functions
            .SingleOrDefault(x =>
            x.Matches(name, returnType, paramList));

        if (CurrentFunction != null) return true;

        return false;
        //    new FunctionExpressionToken<TValidationType>(
        //    CurrentFunction
        //);
    }

    public void SetCurrentParam(string? name, Type paramTp)
    {
        if (CurrentFunction == null) throw new Exception();

        //var paramDescriptorType = typeof(ParamDescriptor<,>).MakeGenericType(paramTp, typeof(TValidationType));
        //CurrentParam = Activator.CreateInstance(paramDescriptorType, new object[] { name ?? string.Empty, ((IMainValidator<TValidationType>)this).Store, CurrentFunction }) as IParamDescriptor;
        CurrentParam = CurrentFunction?.Params?.FirstOrDefault(x => x.Matches(name ?? "", paramTp)) ?? null;
    }

    //public void AddValidations(IEnumerable<IParamValidation> validations)
    //{
    //    if (CurrentFunction != null && CurrentParam != null)
    //    {
    //        CurrentParam.Validations.AddRange(validations);
    //    }
    //    else throw new Exception();
    //}
}
