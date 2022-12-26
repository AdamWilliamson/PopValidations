using System.Threading.Tasks;
using PopValidations.FieldDescriptors.Base;

namespace PopValidations.Validations.Base;

public interface IScopeData
{
    void ReHome(IFieldDescriptorOutline fieldDescriptorOutline);
    Task Init(object? instance);
    object? GetValue();
    string Describe();
    void SetParent(IScopeData parent);
}
