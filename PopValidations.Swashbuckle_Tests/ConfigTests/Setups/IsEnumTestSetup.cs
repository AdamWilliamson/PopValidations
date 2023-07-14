using PopValidations.Swashbuckle_Tests.Helpers;

namespace PopValidations.Swashbuckle_Tests.ConfigTests.Setups;

public static class IsEnumTestSetup
{
    public class TestController : ControllerBase<Request> { }

    public record Request(int? Id, string? Name, SubRequest Child);
    public record SubRequest(int? Id, string? Name);

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