using PopValidations.Swashbuckle;

namespace PopValidations.Swashbuckle_Tests.DisabledTests
{
    public class DisabledConfig : OpenApiConfig
    {
        public DisabledConfig()
        {
            TypeValidationLevel = (Type t) => ValidationLevel.None;
        }
    }
}
