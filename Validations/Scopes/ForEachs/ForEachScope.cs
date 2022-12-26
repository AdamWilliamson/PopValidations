using System;
using System.Collections.Generic;
using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Scopes.ForEachs;

internal class ForEachScope<TValidationType, TFieldType> : ScopeBase
{
    private readonly IFieldDescriptor<TValidationType, IEnumerable<TFieldType>> fieldDescriptor;
    private Action<IFieldDescriptor<IEnumerable<TFieldType>, TFieldType>> actions;

    public ForEachScope(
        IFieldDescriptor<TValidationType, IEnumerable<TFieldType>> fieldDescriptor,
        Action<IFieldDescriptor<IEnumerable<TFieldType>, TFieldType>> actions
    ) : base(fieldDescriptor.Store)
    {
        this.fieldDescriptor = fieldDescriptor;
        this.actions = actions;
    }

    public override string Name => nameof(ForEachScope<TValidationType, TFieldType>);

    protected override void InvokeScopeContainer(ValidationConstructionStore store, object? value)
    {
        if (value == null)
        {
            return;
        }
        if (value is IEnumerable<TFieldType> list)
        {
            int index = 0;

            foreach (var item in list)
            {
                var thingo = new FieldDescriptor<IEnumerable<TFieldType>, TFieldType>(
                    new IndexedPropertyExpressionToken<IEnumerable<TFieldType>, TFieldType>(
                        fieldDescriptor.PropertyToken.Name + $"[{index}]",
                        index
                    ),
                    store
                );

                store.AddItem(
                    thingo,
                    new ForEachIndexedScope<TValidationType, TFieldType>(store, thingo, actions)
                );

                if (this.IsVital)
                {
                    var thingo2 = new FieldDescriptor<IEnumerable<TFieldType>, TFieldType>(
                        new IndexedPropertyExpressionToken<IEnumerable<TFieldType>, TFieldType>(
                            fieldDescriptor.PropertyToken.Name
                                + $"[{index}]"
                                + Char.MaxValue,
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

        store.AddItem(
            thingo,
            new ForEachIndexedScope<IEnumerable<TFieldType>, TFieldType>(store, thingo, actions)
        );
    }
}
