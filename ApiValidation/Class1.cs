using PopValidations.Execution;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiValidation;

//public class ApiValidationException : Exception 
//{
//    public ApiValidationException() : base() { }
//    public ApiValidationException(string message) : base(message) { }
//}

//public class ApiValidationInvalidParameterException : ApiValidationException 
//{
//    public ApiValidationInvalidParameterException(string message)
//        : base(message)
//    { }
//}

//public class FunctionInfo
//{
//    public string Function { get; }
//    List<MethodParamInfo> Params = new();

//    public FunctionInfo(string functionString)
//    {
//        Function = functionString;
//    }

//    public void AddMemberParamInfo(MethodParamInfo paramInfo)
//    {
//        Params.Add(paramInfo);
//    }

//    public List<MethodParamInfo> GetParams() { return Params.ToList(); }
//}

//public class MethodParamInfo
//{
//    public string Method { get; init; }
//    public int Ordinal { get; init; }
//    public Type ParamType { get; init; }
//    private List<IParamValidation> validations = new();

//    public MethodParamInfo(string method, int paramOrdinal, Type paramType)
//    {
//        this.Method = method;
//        this.Ordinal = paramOrdinal;
//        ParamType = paramType;
//    }

//    public List<IParamValidation> GetValidations() 
//    {
//        return validations.ToList();
//    }
//}

//public class ParamValidationDescription
//{
//    public string Description { get; }

//    public ParamValidationDescription(string description)
//    {
//        Description = description;
//    }
//}

//public interface IParamValidation 
//{
//    ParamValidationDescription Describe();
//}

//public static class MemberValidatorExtensions
//{
//    public static IMemberValidator IsNotNull(this IMemberValidator validator)
//    {
//        //validator.AddValidation(new IsNotNullParamValidation());
//        return validator;
//    }
//}

//public class Validator<T> : IValidator<T>
//{
//    protected ApiScope Scope = new();

//    public Validator()
//    {

//    }

    

//    public ApiScope GetScope() { return Scope; }
//}

//public static class Param
//{
//    public static T Is<T>() { return default(T); }
//}

//public class ApiScope
//{
//    private Dictionary<string, FunctionInfo> functions = new();
//    private List<(MethodParamInfo methodInfo, IParamValidation validation)> methodValidations = new();

//    public void AddValidation(MethodParamInfo paramInfo, IParamValidation validation) 
//    {
//        if (!functions.ContainsKey(paramInfo.Method))
//        {
//            functions.Add(paramInfo.Method, new FunctionInfo(paramInfo.Method));
//        }

//        functions[paramInfo.Method].AddMemberParamInfo(paramInfo);
//    }

//    public List<(MethodParamInfo methodInfo, IParamValidation validation)> GetValidations()
//    {
//        return methodValidations;
//    }

//    public List<FunctionInfo> GetApiValidations()
//    {
//        return functions.Values.ToList();
//    }
//}

//public interface IValidator<T> 
//{
//    ApiScope GetScope();
//}

//public class ParamResult 
//{
//    public int Ordinal { get; }
//    public Type Type { get; }
//    public ParamValidationDescription ValidationDescription { get; }

//    public ParamResult(int ordinal, Type type, ParamValidationDescription validationDescription)
//    {
//        Ordinal = ordinal;
//        Type = type;
//        ValidationDescription = validationDescription;
//    }
//}

//public class FunctionResult
//{
//    public string Function { get; }
//    public List<ParamResult> ParamResults { get; } = new();

//    public FunctionResult(string function)
//    {
//        Function = function;
//    }
//}

//public class ApiValidationResult
//{
//    public Dictionary<string, FunctionResult> Functions { get; } = new();

//    public void Add(FunctionInfo member, MethodParamInfo param, ParamValidationDescription description)
//    {
//        if (!Functions.ContainsKey(member.Function))
//        {
//            Functions.Add(member.Function, new FunctionResult(member.Function));
//        }

//        Functions[member.Function].ParamResults.Add(new ParamResult(param.Ordinal, param.ParamType, description));
//    }
//}

//public class ApiDescriptionResult
//{

//}

//public interface IValidationRunnerFactory
//{
//    public IValidationRunner<T> GetRunnerFor<T>();
//}

//public class ApiValidatorRunner<T>
//{
//    private readonly IValidationRunnerFactory runnerFactory;
//    private readonly IEnumerable<IValidator<T>> validators;

//    public ApiValidatorRunner(
//        IValidationRunnerFactory runnerFactory, 
//        IEnumerable<IValidator<T>> validators)
//    {
//        this.runnerFactory = runnerFactory;
//        this.validators = validators;
//    }

//    public ApiValidationResult Validate() 
//    {
//        return new ApiValidationResult(); 
//    }

//    public ApiDescriptionResult Describe() 
//    {

//        var result = new ApiValidationResult();

//        foreach (var validator in validators)
//        {
//            foreach (var member in validator.GetScope().GetApiValidations())
//            {
//                foreach(var param in member.GetParams())
//                {
//                    foreach(var validation in param.GetValidations())
//                    {
//                        var description = validation.Describe();
//                        result.Add(member, param, description);
//                    }
//                }

                
//            }
//        }

//        return new ApiDescriptionResult(); 
//    }
//}