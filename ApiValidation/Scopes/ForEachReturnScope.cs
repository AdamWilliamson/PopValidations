using ApiValidations.Descriptors;
using ApiValidations.Descriptors.Core;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors;
using PopValidations.FieldDescriptors.Base;
using PopValidations.Scopes;
using PopValidations.Validations.Base;
using PopValidations.ValidatorInternals;
using System.Diagnostics;

namespace ApiValidations.Scopes;

public class ForEachReturnDescriptor<TEnumeratedFieldType, TReturnType>
    : IReturnDescriptor<TReturnType>, IFieldDescriptorOutline, IReturnDescriptor_Internal
{
    protected object? RetrievedValue = null;
    protected bool ValueHasBeenRetrieved = false;
    private readonly IValidationStore store;
    private readonly int index;

    protected bool _NextValidationVital { get; set; } = false;
    protected bool _AlwaysVital { get; set; } = false;
    public string PropertyName => (_functionDescriptor.Name ?? string.Empty) + $"::Return({typeof(TReturnType).Name})[{((index >= 0) ? index.ToString() : 'n')}]";

    IFunctionExpressionToken _functionDescriptor { get; set; }
    IFunctionExpressionToken IReturnDescriptor_Internal.FunctionDescriptor => _functionDescriptor;

    //WhenReturningNotValidatingValidator<TValidationType, TReturnType> when;

    public ForEachReturnDescriptor(IValidationStore store, IFunctionExpressionToken functionDescription, int index)
    {
        this.store = store;
        _functionDescriptor = functionDescription;
        this.index = index;
        //when = new WhenReturningNotValidatingValidator<TValidationType, TReturnType>(store, this);
        //store.AddItem(null, when);
    }

    public virtual string AddTo(string existing)
    {
        return _functionDescriptor.CombineWithParentProperty(existing) + $"::Return({typeof(TReturnType).Name})[{((index >= 0)? index.ToString() : 'n')}]";
    }

    public void AddValidation(IValidationComponent validation)
    {
        //store.AddItem()
        //..when.AddValidation(_NextValidationVital || _AlwaysVital, validation);
        store.AddItem(_NextValidationVital || _AlwaysVital, this, validation);
        _NextValidationVital = false;
    }

    public IReturnDescriptor<TReturnType> NextValidationIsVital()
    {
        _NextValidationVital = true;
        return this;
    }

    public IReturnDescriptor<TReturnType> SetAlwaysVital()
    {
        _AlwaysVital = true;
        return this;
    }

    public void AddSubValidator(ISubValidatorClass<TReturnType> component)
    {
        foreach (var item in component.Store.GetItems())
        {
            store.AddItemToCurrentScope(this, item);
        }

        component.ChangeStore(store);

        _NextValidationVital = false;
    }

    public void AddSelfDescribingEntity(IExpandableEntity component)
    {
        if (_NextValidationVital || _AlwaysVital) component.AsVital();

        store.AddItem(
            null,
            component
        );
        _NextValidationVital = false;
    }

    public virtual object? GetValue(object? value)
    {
        return null;
    }
}

internal class ForEachReturnScope<TReturnType> : ScopeBase
{
    private readonly IFunctionExpressionToken functionExpressionToken;
    private readonly IReturnDescriptor<IEnumerable<TReturnType>> returnDescriptor;
    private readonly Action<IReturnDescriptor<TReturnType>> actions;

    public override bool IgnoreScope => true;

    public ForEachReturnScope(
        IFunctionExpressionToken functionExpressionToken,
        IReturnDescriptor<IEnumerable<TReturnType>> returnDescriptor,
        Action<IReturnDescriptor<TReturnType>> actions
    )
    {
        this.functionExpressionToken = functionExpressionToken;
        this.returnDescriptor = returnDescriptor ?? throw new Exception("Wrong type of field descriptor");
        this.actions = actions;
    }

    public override string Name => nameof(ForEachReturnScope<TReturnType>);

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        throw new NotImplementedException();
        //if (value is not TReturnType converted)
        //{
        //    return;
        //}

        //var potentialList = returnDescriptor.PropertyToken.Execute(converted);
        //if (potentialList is IEnumerable<TReturnType> list)
        //{
        //    int index = 0;

        //    foreach (var item in list)
        //    {
        //        var thingo = new ForEachReturnDescriptor<IEnumerable<TReturnType>, TReturnType>(
        //            fieldDescriptor.PropertyToken,
        //            new IndexedPropertyExpressionToken<IEnumerable<TReturnType>, TReturnType>(
        //                fieldDescriptor.PropertyToken.Name + $"[{index}]",
        //                index
        //            ),
        //            store
        //        );
        //        try
        //        {
        //            actions.Invoke(thingo);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex);
        //        }

        //        if (this.IsVital)
        //        {
        //            var thingo2 = new ForEachFieldDescriptor<IEnumerable<TReturnType>, TReturnType>(
        //            fieldDescriptor.PropertyToken,
        //            new IndexedPropertyExpressionToken<IEnumerable<TReturnType>, TReturnType>(
        //                fieldDescriptor.PropertyToken.Name + $"[{index}" + Char.MaxValue,
        //                index
        //            ),
        //            store
        //        );

        //            thingo2.NextValidationIsVital();

        //            thingo2.AddValidation(
        //                new VitallyForEachValidation(fieldDescriptor.PropertyToken.Name + $"[")
        //            );
        //        }

        //        index++;
        //    }
        //}
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        var thingo = new ForEachReturnDescriptor<IEnumerable<TReturnType>, TReturnType>(
           store,
           functionExpressionToken,
           -1
        );

        actions.Invoke(thingo);
        //thingo.InvokeScopeContainerToDescribe();
    }

    public override void ChangeStore(IValidationStore store) { }
}