namespace ApiValidations.Descriptors.Core;

public interface IFunctionDescriptor_Internal
{
    Type? ReturnType { get; }
    string Name { get; }
    IEnumerable<ParamDetailsDTO>? ParamList { get; }
}
