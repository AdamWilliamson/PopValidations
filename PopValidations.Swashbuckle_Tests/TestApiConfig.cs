using PopValidations.Swashbuckle;

namespace PopValidations.Swashbuckle_Tests
{
    public class TestApiConfig : OpenApiConfig
    {
        public TestApiConfig()
        {
            TypeValidationLevel = (Type t) =>
                (
                    ValidationLevel.None
                );
        }
    }
}
