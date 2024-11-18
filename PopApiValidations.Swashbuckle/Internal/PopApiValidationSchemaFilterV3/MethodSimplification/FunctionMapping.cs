using System.Reflection;

namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

public class FunctionMapping
{
    public MethodInfo MethodInfo { get; set; }
    public string OpenApiPath { get; set; }
    public string OpenApiOperation { get; set; }
    public List<ParameterMapping> Parameters { get; set; } = new();
    public List<ReturnMapping> Return { get; set; } = new();
}
