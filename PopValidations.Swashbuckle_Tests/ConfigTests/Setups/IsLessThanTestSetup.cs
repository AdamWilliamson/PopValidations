using PopValidations.Swashbuckle_Tests.Helpers;

namespace PopValidations.Swashbuckle_Tests.ConfigTests.Setups;

public static class IsLessThanTestSetup
{
    public class TestController : ControllerBase<Request> { }

    public record Request(int? Id, string? Name, SubRequest Child);
    public record SubRequest(int? Id, string? Name);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            Describe(x => x.Id).IsLessThan(5);
            Describe(x => x.Name).Vitally().IsLessThan("Test");
            Describe(x => x.Child).SetValidator(new SubRequestValidator());
        }
    }

    public class SubRequestValidator : AbstractSubValidator<SubRequest>
    {
        public SubRequestValidator()
        {
            Describe(x => x.Id).IsLessThan(6);
            Describe(x => x.Name).Vitally().IsLessThan("Test2");
        }
    }
}