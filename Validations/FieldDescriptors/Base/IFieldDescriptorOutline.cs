namespace PopValidations.FieldDescriptors.Base;

public interface IFieldDescriptorOutline
{
    string PropertyName { get; }
    string AddTo(string existing);
    object? GetValue(object? input);
}
