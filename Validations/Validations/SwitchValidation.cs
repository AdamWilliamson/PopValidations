using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PopValidations.Validations;

public interface ISwitchValidator<TValidationType, TScopedDataType>
{
    ISwitchValidator<TValidationType, TScopedDataType> Case<TFieldType>(
        Expression<Func<TValidationType, TFieldType?>> Describe, 
        string Description, 
        Func<TFieldType?, TScopedDataType?, bool> Test, 
        string ErrorMessage);

    ISwitchValidator<TValidationType, TScopedDataType> Ignore(string description, Func<TValidationType?, TScopedDataType?, bool> test);
}

public sealed class SwitchScope<TValidationType, TScopedDataType> : ScopeBase, ISwitchValidator<TValidationType, TScopedDataType>
{
    private readonly AbstractValidatorBase<TValidationType> parent;
    private readonly IScopedData<TScopedDataType?> scopedData;

    public override bool IgnoreScope => true;
    public override string Name => scopedData.Describe();

    public SwitchScope(
        AbstractValidatorBase<TValidationType> parent,
        IScopedData<TScopedDataType?> scopedData
    )
    {
        this.parent = parent;
        this.scopedData = scopedData;
    }

    public override void ReHomeScopes(IFieldDescriptorOutline fieldDescriptorOutline)
    {
        scopedData.ReHome(fieldDescriptorOutline);
    }

    protected override async void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        if (FieldDescriptor != null)
            await scopedData.Init(FieldDescriptor.GetValue(value));
        else
            await scopedData.Init(value);
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store){}

    public override void ChangeStore(IValidationStore store) { }

    public ISwitchValidator<TValidationType, TScopedDataType> Case<TFieldType>(
        Expression<Func<TValidationType, TFieldType?>> Describe, 
        string Description, 
        Func<TFieldType?, TScopedDataType?, bool> Test, 
        string ErrorMessage)
    {
        var fieldDescriptor = parent.Describe(Describe);

        var validation = new IsCustomValidation<TFieldType>(
            Description,
            ErrorMessage,
            new ScopedData<TFieldType?, bool>("",
                (x) => {

                    Debug.Assert(x is TFieldType || x is TFieldType? || x is null);
                    Debug.Assert(scopedData?.GetValue() is TScopedDataType || scopedData?.GetValue() is null);
                    var data = scopedData?.GetValue();

                    if (data is TScopedDataType)
                        return !(Test?.Invoke(x, (TScopedDataType)data) ?? true);
                    else if (data is null)
                        return !(Test?.Invoke(x, default) ?? true);
                    else
                        throw new ScopedDataException("Switch");
                }
            )
        );

        fieldDescriptor.AddValidation(validation);

        return this;
    }

    public ISwitchValidator<TValidationType, TScopedDataType> Ignore(string description, Func<TValidationType?, TScopedDataType?, bool> test)
    {
        return this;
    }
}
