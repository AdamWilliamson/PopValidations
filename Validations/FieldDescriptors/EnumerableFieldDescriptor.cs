using PopValidations.Execution.Stores;
using PopValidations.FieldDescriptors.Base;
using System.Collections.Generic;

namespace PopValidations.FieldDescriptors;

public class EnumerableFieldDescriptor<TValidationType, TFieldType> : FieldDescriptor<TValidationType, IEnumerable<TFieldType>?> 
{
    public EnumerableFieldDescriptor(
        IPropertyExpressionToken<IEnumerable<TFieldType>?> propertyToken,
        IValidationStore store
    ) : base(propertyToken, store) { }
}
