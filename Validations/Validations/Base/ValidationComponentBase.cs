using System;
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

    public virtual DescribeActionResult Describe()
    {
        return CreateDescription();
    }

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
        //string GetNameWithoutGenericArity(Type t)
        //{
        //    string name = t.Name;
        //    int index = name.IndexOf('`');
        //    return index == -1 ? name : name.Substring(0, index);
        //}

        string GetNameWithoutGenericArity(Type? t)
        {
            if (t == null) return string.Empty;

            string name = t.Name;
            int index = name.IndexOf('`');
            name = index == -1 ? name : name.Substring(0, index);

            if (t.IsGenericType)
            {
                name += $"<{string.Join(',', t.GenericTypeArguments.ToList().Select(x => GetNameWithoutGenericArity(x)).ToList())}>";
            }

            return name;
        }

        return new DescribeActionResult(
            validator: GetNameWithoutGenericArity(this.GetType()),
            message: DescriptionTemplate,
            keyValues
                .Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value))
                .ToList()
        );
    }
}
