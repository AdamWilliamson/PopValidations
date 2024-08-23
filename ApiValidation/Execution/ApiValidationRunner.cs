using System.Collections;
using System.Reflection;
using PopValidations.Execution;
using PopValidations.Execution.Description;
using PopValidations.Execution.Validation;

namespace ApiValidations.Execution;

public interface IApiValidationDescriber
{
    DescriptionResult Describe();
}

public interface IApiValidationRunner<TValidationType>: IApiValidationDescriber
{
    Task<ValidationResult> Validate(TValidationType instance, HeirarchyMethodInfo methodInfo);
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

public class ApiValidationRunner<TValidationType> : IApiValidationRunner<TValidationType>, IApiValidationDescriber
{
    private readonly IValidationRunner<TValidationType> validationRunner;
    //private readonly IValidatorCreationFactory validatorCreationFactory;
    private readonly IEnumerable<IApiMainValidator<TValidationType>> mainValidators;

    public ApiValidationRunner(
        IValidationRunner<TValidationType> validationRunner,
        //IValidatorCreationFactory validatorCreationFactory,
        IEnumerable<IApiMainValidator<TValidationType>> mainValidators
        )
    {
        this.validationRunner = validationRunner;
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

    public async Task<ValidationResult> Validate(TValidationType instance, HeirarchyMethodInfo methodInfo)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));

        // Run Validations on Object's Function's Properties.
        foreach(var mainValidator in mainValidators)
        {
            mainValidator.SetCurrentExecutionContext(methodInfo);
        }

        // Old code trying to run Validate() on the child object only.. Mistake., 
        //var creationMethod = validatorCreationFactory.GetType().GetMethod(nameof(IValidatorCreationFactory.CreateFor))?.GetGenericMethodDefinition();
        //var finalMethod = creationMethod?.MakeGenericMethod(resultantObject.GetType());

        //if (finalMethod == null) { throw new Exception("Function Doesnt exist to validate"); }
        //finalMethod.Invoke(resultantObject, methodInfo.ParamValues.ToArray());

        var objectGraphWithFunction = (string.IsNullOrWhiteSpace(methodInfo.ObjectMap))
            ? methodInfo.Method.Name + "("
            : string.Join('.', [methodInfo.ObjectMap, methodInfo.Method.Name + "("]);

        var validations = await validationRunner.Validate(instance, [objectGraphWithFunction]);

        if (validations.Errors.Any())
        {
            return validations;
        }

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


        // Validate the Return value

        // if Successful Return Value

        return new ValidationResult();
    }

    public DescriptionResult Describe() 
    {
        return validationRunner.Describe();
    }
}
