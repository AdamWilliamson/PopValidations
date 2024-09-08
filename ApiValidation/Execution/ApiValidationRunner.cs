using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Principal;
using ApiValidations.Helpers;
using PopValidations.Configurations;
using PopValidations.Execution;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validation;
using PopValidations.ValidatorInternals;

namespace ApiValidations.Execution;

public interface IApiValidationDescriber
{
    DescriptionResult Describe();
}

public interface IApiValidationRunner<TValidationType>: IApiValidationDescriber
{
    Task<ApiValidationResult> Validate(TValidationType instance, HeirarchyMethodInfo methodInfo);
    Task<ApiValidationResult> ValidateAndExecute(TValidationType instance, HeirarchyMethodInfo methodInfo);
    Task<ValidationResult> Validate(TValidationType instance);
}

public interface IValidatorCreationFactory
{
    IValidationRunner<TValidationType> CreateFor<TValidationType>();
}

public class HeirarchyMethodInfo
{
    public HeirarchyMethodInfo(string objectMap, MethodInfo method, List<object> paramValues)
    {
        ObjectMap = objectMap.Trim('.');
        Method = method;
        ParamValues = paramValues;
    }

    public string ObjectMap { get; }
    public MethodInfo Method { get; }
    public List<object> ParamValues { get; }
}

public class HeirarchyReturnMethodInfo
{
    public HeirarchyReturnMethodInfo(HeirarchyMethodInfo info, object? returnValue)
    {
        ReturnValue = returnValue;
        ObjectMap = info.ObjectMap.Trim('.');
        Method = info.Method;
        ParamValues = info.ParamValues;
    }

    public object? ReturnValue { get; }
    public string ObjectMap { get; }
    public MethodInfo Method { get; }
    public List<object> ParamValues { get; }
}

public class ApiConfiguration
{
    //public LangaugeConfiguration Language { get; set; } = new();

    public string ParamToken { get; set; } = ":Param";
    public string ReturnToken { get; set; } = ":Return";

    public Func<MethodInfo, string> FunctionDescription { get; set; } = (methodInfo) =>
    {
        var paramList = methodInfo.GetParameters();
        var returnType = methodInfo.ReturnType;
        return methodInfo.Name + $"({string.Join(',', paramList?.Select(x => GenericNameHelper.GetNameWithoutGenericArity(x.ParameterType)) ?? [])})->{GenericNameHelper.GetNameWithoutGenericArity(returnType)}";
    };

    public Func<MethodInfo, int, string> ParamDescription { get; set; }
    public Func<Type, string> ReturnDescription { get; set; }
    public Func<MethodInfo, int, int?, string> DescribeValidatingParam { get; set; }
    public Func<MethodInfo, int?, string> DescribeValidatingReturn { get; set; }
    public Func<string, string> GetParamNameFromErrorKey { get; set; }

    public ApiConfiguration()
    {
        ParamDescription = (MethodInfo methodInfo, int paramIndex) =>
        {
            var param = methodInfo.GetParameters()[paramIndex];
            return $"{ParamToken}({param.Position},{GenericNameHelper.GetNameWithoutGenericArity(param.ParameterType)},{param.Name})";
        };

        ReturnDescription = (Type returnType) =>
        {
            return $"{ReturnToken}({GenericNameHelper.GetNameWithoutGenericArity(returnType)})";
        };

        DescribeValidatingParam = (MethodInfo methodInfo, int paramIndex, int? indexArray) =>
        {
            var description = $"{FunctionDescription.Invoke(methodInfo)}{ParamDescription.Invoke(methodInfo, paramIndex)}";
            if (indexArray.HasValue)
            {
                description += $"[{(indexArray >= 0 ? indexArray.ToString() : 'n')}]";
            }

            return description;
        };

        DescribeValidatingReturn = (MethodInfo methodInfo, int? returnIndex) =>
        {
            var description = $"{FunctionDescription.Invoke(methodInfo)}{ReturnDescription.Invoke(methodInfo.ReturnType)}";
            if (returnIndex.HasValue)
            {
                description += $"[{(returnIndex >= 0 ? returnIndex.ToString() : 'n')}]";
            }

            return description;
        };

        GetParamNameFromErrorKey = (string funcCall) =>
        {
            var pieces = funcCall.Split('.');
            int paramTokenIndex = -1;
            int substringStart = -1;

            for (var x = 0; x < pieces.Count(); x++)
            {
                substringStart = pieces[x].IndexOf(ParamToken);
                if (substringStart >= 0)
                {
                    paramTokenIndex = x;
                    substringStart += ParamToken.Length;
                    break;
                }
            }

            pieces[paramTokenIndex] = pieces[paramTokenIndex].Substring(substringStart + 1);
            int endSubstring = pieces[paramTokenIndex].IndexOf(')');
            pieces[paramTokenIndex] = pieces[paramTokenIndex].Substring(0, endSubstring);
            string paramName = pieces[paramTokenIndex].Split(',')[2];

            var contents = pieces.Skip(paramTokenIndex + 1).ToList();
            contents.Insert(0, paramName);
            paramName = string.Join('.', contents);
            return paramName;
        };
    }
}

public class ApiValidationResult : ValidationResult
{
    [IgnoreDataMember]
    public object? Result { get; set; }

    public ApiValidationResult() { }

    public ApiValidationResult(ValidationResult validationResult)
    {
        Errors = validationResult.Errors;
    }

    public ApiValidationResult(ValidationResult validationResult, object? result)
        : this(validationResult)
    {
        Result = result;
    }
}

public class ApiValidationResult<T> : ValidationResult
{
    public T? Result { get; set; }
    public ApiValidationResult() { }
    public ApiValidationResult(ValidationResult validationResult)
    {
        Errors = validationResult.Errors;
    }
}

public class ApiValidationRunner<TValidationType> : IApiValidationRunner<TValidationType>, IApiValidationDescriber
{
    private readonly IValidationRunner<TValidationType> validationRunner;
    //private readonly IValidatorCreationFactory validatorCreationFactory;
    private readonly IEnumerable<IApiMainValidator<TValidationType>> mainValidators;

    public ApiValidationRunner(
        IEnumerable<IApiMainValidator<TValidationType>> mainValidators,//...  WTF.  how do I get the same validations in SecurityIdentifier the Runner.
        MessageProcessor messageProcessor
        //IValidatorCreationFactory validatorCreationFactory,
        )
    {
        this.validationRunner = new ValidationRunner<TValidationType>(mainValidators.Cast<IMainValidator<TValidationType>>(), messageProcessor);
        //this.validatorCreationFactory = validatorCreationFactory;
        this.mainValidators = mainValidators;
    }

    public async Task<ValidationResult> Validate(TValidationType instance)
    {
        return await validationRunner.Validate(instance);
    }

    private object? EnumerateValue(IEnumerable items, int index)
    {
        var enumerator = items.GetEnumerator();

        for (int x = 0; x <= index; x++)
        {
            if (x == index)
            {
                return enumerator.Current;
            }
            enumerator.MoveNext();
        }
        throw new Exception("Index is larger than array length");
    }

    private object? GetIndexedPropertyValue(object? property, string index)
    {
        return property switch
        {
            Array a => a.GetValue(int.Parse(index)),
            IList l => l[int.Parse(index)],
            IDictionary d => d[index],
            IEnumerable e => EnumerateValue(e, int.Parse(index)),
            _ => throw new Exception("Not a known Array Type")
        };
    }

    private object? GetPropertyByString(object obj, string propertyName)
    {
        if (propertyName.Contains("[") && propertyName.Contains("]"))
        {
            var splitItems = propertyName.Split('[');
            var nonIndexedPropertyName = splitItems[0];
            var index = splitItems[1].Split(']')[0];
            var array = obj?.GetType().GetProperty(nonIndexedPropertyName)?.GetValue(obj);

            return GetIndexedPropertyValue(array, index);
        }
        else 
        { 
            return obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
        }
    }

    public async Task<ApiValidationResult> Validate(TValidationType instance, HeirarchyMethodInfo methodInfo)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));

        // Run Validations on Object's Function's Properties.
        foreach (var mainValidator in mainValidators)
        {
            mainValidator.SetCurrentExecutionContext(methodInfo);
        }

        var funcDesc = PopApiValidations.Configuation.FunctionDescription(methodInfo.Method);
        var objectGraphWithFunction = (string.IsNullOrWhiteSpace(methodInfo.ObjectMap))
            ? funcDesc
            : string.Join('.', [methodInfo.ObjectMap, funcDesc]);
        var objectGraphWithFunctionAndParam = objectGraphWithFunction + PopApiValidations.Configuation.ParamToken;
        var validations = await validationRunner.Validate(instance, [objectGraphWithFunctionAndParam]);

        return new ApiValidationResult(validations);
    }

    public async Task<ApiValidationResult> ValidateAndExecute(TValidationType instance, HeirarchyMethodInfo methodInfo)
    {
        var validations = await Validate(instance, methodInfo);

        if (validations.Errors.Any())
        {
            return new ApiValidationResult(validations);
        }

        var funcDesc = PopApiValidations.Configuation.FunctionDescription(methodInfo.Method);
        var objectGraphWithFunction = (string.IsNullOrWhiteSpace(methodInfo.ObjectMap))
            ? funcDesc
            : string.Join('.', [methodInfo.ObjectMap, funcDesc]);

        // Navigate to Object
        var Properties = methodInfo.ObjectMap.Split('.').Where(x => !string.IsNullOrWhiteSpace(x));
        object? resultantObject = instance;

        if (Properties.Any())
        {
            foreach (var prop in Properties)
            {
                resultantObject = GetPropertyByString(resultantObject, prop);
                if (resultantObject == null) throw new MemberAccessException($"{prop} not found on child object");
            }
        }

        // Execute Function on Object
        var returnDesc = PopApiValidations.Configuation.ReturnDescription(methodInfo.Method.ReturnType);
        objectGraphWithFunction += returnDesc;

        //===
        if (methodInfo.Method.ReturnType != typeof(void))
        {
            object? result = methodInfo.Method.Invoke(resultantObject, methodInfo.ParamValues.ToArray());
            var completedMethodInfo = new HeirarchyReturnMethodInfo(methodInfo, result);

            foreach (var mainValidator in mainValidators)
            {
                mainValidator.SetCurrentExecutionContext(completedMethodInfo);
            }

            var returnValidations = await validationRunner.Validate(instance, [objectGraphWithFunction]);
            return new ApiValidationResult(returnValidations, result);
        }
        
        methodInfo.Method.Invoke(resultantObject, methodInfo.ParamValues.ToArray());
        return new ApiValidationResult();
    }

    public DescriptionResult Describe() 
    {
        return validationRunner.Describe();
    }
}
