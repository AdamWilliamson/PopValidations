using PopValidations.Swashbuckle;

namespace PopValidations.Swashbuckle_Tests.DisabledTests;

public class SimpleFilterConfig : OpenApiConfig
{
    public SimpleFilterConfig(Type skippedType)
    {
        TypeValidationLevel = (Type t) => 
            t == skippedType
                ? ValidationLevel.None 
                : ValidationLevel.FullDetails;
    }
}