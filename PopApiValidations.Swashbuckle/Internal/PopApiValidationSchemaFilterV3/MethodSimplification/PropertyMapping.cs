namespace PopApiValidations.Swashbuckle.Internal.PopApiValidationSchemaFilterV3.MethodSimplification;

public class PropertyMapping
{
    public Type PropertyType { get; set; }
    public string PropertyName { get; set; }
    public string OpenApiPropertyName { get; set; }
    public string? ResultPropertyName { get; set; }
    public List<PropertyMapping> Properties { get; set; } = new();
    public bool IsArrayType { get; set; } = false;
}
