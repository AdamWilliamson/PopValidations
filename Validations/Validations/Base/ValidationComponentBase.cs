using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Validations.Base;

public abstract class ValidationComponentBase : IValidationComponent
{
    public abstract string DescriptionTemplate { get; protected set; }
    public abstract string ErrorTemplate { get; protected set; }

    public virtual void SetDescriptionTemplate(string desc) { DescriptionTemplate = desc; }
    public virtual void SetErrorTemplate(string error) { ErrorTemplate = error; }

    public ValidationComponentBase() { }

    public virtual void ReHomeScopes(IFieldDescriptorOutline attemptedScopeFieldDescriptor) { }
    public virtual Task InitScopes(object? instance) { return Task.CompletedTask; }
    public abstract ValidationActionResult Validate(object? value);

    public abstract DescribeActionResult Describe();

    protected virtual TResult? GetData<TResult>(IScopeData? scopedData, TResult? value)
    {
        return scopedData switch
        {
            null => value,
            _ => (TResult?)scopedData.GetValue()
        };
    }

    protected virtual string GetDataDescription<TResult>(IScopeData? scopedData, TResult? value)
    {
        return scopedData switch
        {
            null => value?.ToString() ?? "null",
            _ => scopedData.Describe()
        };
    }

    protected ValidationActionResult CreateValidationSuccessful()
    {
        return ValidationActionResult.Successful(this.GetType().Name);
    }

    protected ValidationActionResult CreateValidationError(
        params (string Key, string Value)[] keyValues
        )
    {
        return new ValidationActionResult(
            validator: this.GetType().Name,
            success: false,
            message: ErrorTemplate,
            keyValues
                .Select(kvp => new KeyValuePair<string,string>(kvp.Key, kvp.Value))
                .ToList()
        );
    }

    protected DescribeActionResult CreateDescription(params (string Key, string Value)[] keyValues)
    {
        return new DescribeActionResult(
            validator: this.GetType().Name,
            message: DescriptionTemplate,
            keyValues
                .Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value))
                .ToList()
        );
    }
}
