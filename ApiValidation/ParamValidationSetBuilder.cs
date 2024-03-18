using PopValidations.ValidatorInternals;
using System.Reflection;

namespace PopValidations_Functional_Testbed;

public class ParamValidationSetBuilder<TValidationType>
{
    private List<IFunctionDescriptionFor<TValidationType>> Functions = new();
    public IFunctionDescriptionFor<TValidationType>? CurrentFunction = null;
    public ParamDetailsDTO? CurrentParam = null;
    private readonly IStoreContainer owner;

    public ParamValidationSetBuilder(IStoreContainer owner)
    {
        this.owner = owner;
    }

    public IFunctionDescriptor<TReturnType> SetCurrentFunction<TReturnType>(MethodInfo methodInfo)
    {
        var funcToken = SetCurrentFunction(
            methodInfo.Name, 
            methodInfo.ReturnType, 
            methodInfo.GetParameters().Select(x => new ParamDetailsDTO(x.Name, x.ParameterType, x.Position)).ToList()
        );
        if (funcToken == null) return CurrentFunction as IFunctionDescriptor<TReturnType>;

        var func = new FunctionDescriptor<TValidationType, TReturnType>(funcToken, owner.Store);
        CurrentFunction = func;
        Functions.Add(CurrentFunction);
        return func;
    }

    public IFunctionDescriptor SetCurrentFunction(MethodInfo methodInfo)
    {
        var funcToken = SetCurrentFunction(
            methodInfo.Name,
            methodInfo.ReturnType,
            methodInfo.GetParameters().Select(x => new ParamDetailsDTO(x.Name, x.ParameterType, x.Position)).ToList()
        );
        if (funcToken == null) return CurrentFunction as IFunctionDescriptor;

        var func = new FunctionDescriptor<TValidationType>(funcToken, owner.Store);
        CurrentFunction = func;
        Functions.Add(CurrentFunction);
        return func;
    }

    public FunctionExpressionToken<TValidationType>? SetCurrentFunction(string name, Type returnType, List<ParamDetailsDTO> paramList)
    {
        CurrentFunction = Functions
            .SingleOrDefault(x =>
            x.Matches(name, returnType, paramList));

        if (CurrentFunction != null) return null;

        return new FunctionExpressionToken<TValidationType>(
            name,
            returnType,
            paramList
        );
    }

    public void SetCurrentParam(string? name, Type paramTp)
    {
        if (CurrentFunction == null) throw new Exception();

        //var paramDescriptorType = typeof(ParamDescriptor<,>).MakeGenericType(paramTp, typeof(TValidationType));
        //CurrentParam = Activator.CreateInstance(paramDescriptorType, new object[] { name ?? string.Empty, ((IMainValidator<TValidationType>)this).Store, CurrentFunction }) as IParamDescriptor;
        CurrentParam = CurrentFunction.FunctionPropertyToken.Params.FirstOrDefault(x => x.Matches(name, paramTp));
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
