using PopValidations.Validations.Base;

namespace ApiValidations.Descriptors.Core;


public interface IFunctionContext
{
    string this[int index] { get; }
}

public class FunctionContext : IFunctionContext
{
    List<IScopeData> parameters = new();

    public FunctionContext(int count)
    {
        for(int i = 0; i < count; i++)
        {
            parameters.Add(new ScopedData<object>("Param" + i, new object()));
        }
    }

    public string this[int index] => throw new NotImplementedException();
}

public interface IReturnDescriptor_Internal
{
    IFunctionExpressionToken FunctionDescriptor { get; }
    IFunctionContext GetContext();
}
