using Newtonsoft.Json;
using PopValidations.Swashbuckle_Tests.Helpers;

namespace PopValidations.Swashbuckle_Tests.ConfigTests.Setups;

public static class IsEnumTestSetup
{
    public class TestController : ControllerBase<Request> { }

    public record Request([property: JsonProperty(Order = 1)] int? Id, [property: JsonProperty(Order = 2)] string? Name, [property: JsonProperty(Order = 3)] SubRequest Child);
    public record SubRequest([property: JsonProperty(Order = 1)] int? Id, [property: JsonProperty(Order = 2)] string? Name);

    public enum IsEnumTestSetupEnum { Value1, Value2 };

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            Describe(x => x.Id).IsEnum(typeof(IsEnumTestSetupEnum));
            Describe(x => x.Name).Vitally().IsEnum(typeof(IsEnumTestSetupEnum));
            Describe(x => x.Child).SetValidator(new SubRequestValidator());
        }
    }

    public class SubRequestValidator : AbstractSubValidator<SubRequest>
    {
        public SubRequestValidator()
        {
            Describe(x => x.Id).IsEnum(typeof(IsEnumTestSetupEnum));
            Describe(x => x.Name).Vitally().IsEnum(typeof(IsEnumTestSetupEnum));
        }
    }
}