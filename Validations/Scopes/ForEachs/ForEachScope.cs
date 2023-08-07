using System;
using System.Collections.Generic;
using System.Diagnostics;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors;
using PopValidations.FieldDescriptors.Base;
using PopValidations.ValidatorInternals;

namespace PopValidations.Scopes.ForEachs;

public class ForEachFieldDescriptor<TValidationType, TEnumeratedFieldType, TFieldType>
    : FieldDescriptor<TEnumeratedFieldType, TFieldType>
{
    private readonly IPropertyExpressionToken<TValidationType, TEnumeratedFieldType> parentPropertyExpressionToken;

    public ForEachFieldDescriptor(
        IPropertyExpressionToken<TValidationType, TEnumeratedFieldType> parentPropertyExpressionToken,
        IPropertyExpressionToken<TEnumeratedFieldType, TFieldType> propertyExpressionToken,
        ValidationConstructionStore store
        )
        : base(propertyExpressionToken, store)
    {
        this.parentPropertyExpressionToken = parentPropertyExpressionToken;
    }

    public override object? GetValue(object? value)
    {
        if (ValueHasBeenRetrieved)
            return RetrievedValue;

        if (value is TValidationType result && result != null)
        {
            var parentConverted = parentPropertyExpressionToken.Execute(result);
            if (parentConverted is TEnumeratedFieldType childresult) 
            {
                RetrievedValue = PropertyToken.Execute(childresult);
                ValueHasBeenRetrieved = true;
            }
            else
            {
                Debug.WriteLine(parentConverted?.ToString());
            }
        }
        else
        {
            Debug.WriteLine(value?.ToString());
        }
        return RetrievedValue;
    }
}

//internal class ForEachItemSubValidator<TValidationType, TFieldType> : AbstractSubValidator<TFieldType> 
//    where TValidationType : class
//{
//    private readonly IFieldDescriptor<TValidationType, IEnumerable<TFieldType?>?> fieldDescriptor;

//    //public override bool IgnoreScope => true;
//    public ForEachItemSubValidator(
//        IFieldDescriptor<TValidationType, IEnumerable<TFieldType?>?> fieldDescriptor,
//        Action<IFieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>> actions
//        )
//    {
//        fieldDescriptor.Store.AddItemToCurrentScope(null, new ForEachScope<TValidationType, TFieldType>(fieldDescriptor, actions, Store));
//        this.fieldDescriptor = fieldDescriptor;
//    }

//    public override void ExpandToValidate(ValidationConstructionStore store, object? value)
//    {
//        if ((value is not TValidationType valiationType)
//            || (fieldDescriptor.PropertyToken.Execute(valiationType) is not IEnumerable<TFieldType?> more))
//        {
//            return;
//        }
//        var expandedItems = Store.ExpandToValidate(more);
//        foreach (var item in expandedItems)
//        {
//            if (item != null)
//                store.AddItemToCurrentScope(item);
//        }
//    }

//    //public void ExpandToDescribe(ValidationConstructionStore store)
//    //{
//    //    var expandedItems = Store.ExpandToDescribe();
//    //    foreach (var item in expandedItems)
//    //    {
//    //        if (item != null)
//    //            store.AddItemToCurrentScope(item);
//    //    }
//    //}
//}

internal class ForEachScope<TValidationType, TFieldType> : ScopeBase//, ISubValidatorClass
    where TValidationType : class
{
    private readonly IFieldDescriptor<TValidationType, IEnumerable<TFieldType?>?> fieldDescriptor;
    private Action<IFieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>> actions;

    public override bool IgnoreScope => true;

    public ForEachScope(
        IFieldDescriptor<TValidationType, IEnumerable<TFieldType?>?> fieldDescriptor,
        Action<IFieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>> actions
        //ValidationConstructionStore store
    ) : base(fieldDescriptor.Store)
    {
        this.fieldDescriptor = fieldDescriptor;
        this.actions = actions;
    }

    public override string Name => nameof(ForEachScope<TValidationType, TFieldType>);

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        if (value is not TValidationType converted)
        {
            return;
        }
        var potentialList = fieldDescriptor.PropertyToken.Execute(converted);
        if (potentialList is IEnumerable<TFieldType> list)
        {
            int index = 0;

            foreach (var item in list)
            {
                //var thingo = new FieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>(
                //    new IndexedPropertyExpressionToken<IEnumerable<TFieldType?>, TFieldType?>(
                //        fieldDescriptor.PropertyToken.Name + $"[{index}]",
                //        index
                //    ),
                //    store
                //);

                //store.AddItemToCurrentScope(
                //    //thingo,
                //    null,
                //    new ForEachIndexedScope<TValidationType, TFieldType>(store, index, actions)
                //);

                var thingo = new ForEachFieldDescriptor<TValidationType,IEnumerable<TFieldType?>, TFieldType?>(
                    fieldDescriptor.PropertyToken,
                    new IndexedPropertyExpressionToken<TValidationType,IEnumerable<TFieldType?>, TFieldType?>(
                        fieldDescriptor.PropertyToken.Name + $"[{index}]",
                        index
                    ),
                    store
                );
                try
                {
                    actions.Invoke(thingo);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                if (this.IsVital)
                {
                    //var thingo2 = new ForEachFieldDescriptor<TValidationType, IEnumerable<TFieldType?>, TFieldType>(
                    //    fieldDescriptor.PropertyToken,
                    //    new IndexedPropertyExpressionToken<TValidationType, IEnumerable<TFieldType?>, TFieldType?>(
                    //        fieldDescriptor.PropertyToken.Name + $"[{index}]" + Char.MaxValue,
                    //        index
                    //    ),
                    //    store
                    //);

                    var thingo2 = new ForEachFieldDescriptor<TValidationType, IEnumerable<TFieldType?>, TFieldType?>(
                    fieldDescriptor.PropertyToken,
                    new IndexedPropertyExpressionToken<TValidationType, IEnumerable<TFieldType?>, TFieldType?>(
                        //fieldDescriptor.PropertyToken.Name + $"[{index}]" + Char.MaxValue,
                        fieldDescriptor.PropertyToken.Name + $"[" + Char.MaxValue,
                        index
                    ),
                    store
                );

                    thingo2.NextValidationIsVital();

                    thingo2.AddValidation(
                        new VitallyForEachValidation(fieldDescriptor.PropertyToken.Name + $"[")
                    );
                }

                index++;
            }
        }
    }

    protected override void InvokeScopeContainerToDescribe(ValidationConstructionStore store)
    {
        //store.AddItem(
        //    thingo,
        //    new ForEachIndexedScope<IEnumerable<TFieldType?>, TFieldType?>(store, thingo, actions)
        //);

        //store.AddItem(
        //    null,
        //    new ForEachIndexedScope<IEnumerable<TFieldType?>, TFieldType?>(store, -1, actions)
        //);

        var thingo = new FieldDescriptor<IEnumerable<TFieldType?>, TFieldType?>(
           new IndexedPropertyExpressionToken<TValidationType, IEnumerable<TFieldType?>, TFieldType?>(
               fieldDescriptor.PropertyToken.Name + $"[n]",
               -1
           ),
           //this.validatorStore
           store
       );

        //store.PushFieldDescriptor(thingo);

        actions.Invoke(thingo);
    }
}
