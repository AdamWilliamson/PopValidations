using System;
using System.Collections.Generic;
using System.Diagnostics;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes.ForEachs;

public class ForEachFieldDescriptor<TValidationType, TEnumeratedFieldType, TFieldType>
    : FieldDescriptor<TEnumeratedFieldType, TFieldType>
{
    private readonly IPropertyExpressionToken<TEnumeratedFieldType> parentPropertyExpressionToken;

    public ForEachFieldDescriptor(
        IPropertyExpressionToken<TEnumeratedFieldType> parentPropertyExpressionToken,
        IPropertyExpressionToken<TFieldType> propertyExpressionToken,
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

internal class ForEachScope<TValidationType, TFieldType> : ScopeBase
    where TValidationType : class
{
    private readonly IFieldDescripor_Internal<TValidationType, IEnumerable<TFieldType>> fieldDescriptor;
    private Action<IFieldDescriptor<IEnumerable<TFieldType>, TFieldType>> actions;

    public override bool IgnoreScope => true;

    public ForEachScope(
        IFieldDescripor_Internal<TValidationType, IEnumerable<TFieldType>> fieldDescriptor,
        Action<IFieldDescriptor<IEnumerable<TFieldType>, TFieldType>> actions
    )
    {
        this.fieldDescriptor = fieldDescriptor ?? throw new Exception("Wrong type of field descriptor");
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
                var thingo = new ForEachFieldDescriptor<TValidationType,IEnumerable<TFieldType>, TFieldType>(
                    fieldDescriptor.PropertyToken,
                    new IndexedPropertyExpressionToken<IEnumerable<TFieldType>, TFieldType>(
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
                    var thingo2 = new ForEachFieldDescriptor<TValidationType, IEnumerable<TFieldType>, TFieldType>(
                    fieldDescriptor.PropertyToken,
                    new IndexedPropertyExpressionToken<IEnumerable<TFieldType>, TFieldType>(
                        fieldDescriptor.PropertyToken.Name + $"[{index}" + Char.MaxValue,
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
        var thingo = new FieldDescriptor<IEnumerable<TFieldType>, TFieldType>(
           new IndexedPropertyExpressionToken<IEnumerable<TFieldType>, TFieldType>(
               fieldDescriptor.PropertyToken.Name + $"[n]",
               -1
           ),
           store
        );

        actions.Invoke(thingo);
    }

    public override void ChangeStore(IValidationStore store) { }
}
