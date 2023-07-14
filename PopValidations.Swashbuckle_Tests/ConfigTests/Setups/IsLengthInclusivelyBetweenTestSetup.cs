using PopValidations.Swashbuckle_Tests.Helpers;

namespace PopValidations.Swashbuckle_Tests.ConfigTests.Setups;

public static class IsLengthInclusivelyBetweenTestSetup
{
    public class TestController : ControllerBase<Request> { }

    public record Request(int? Id, string? Name, SubRequest Child);
    public record SubRequest(int? Id, string? Name);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            Describe(x => x.Id).IsLengthInclusivelyBetween(5, 10);
            Describe(x => x.Name).Vitally().IsLengthInclusivelyBetween(0, 20);
            Describe(x => x.Child).SetValidator(new SubRequestValidator());
        }
    }

    public class SubRequestValidator : AbstractSubValidator<SubRequest>
    {
        public SubRequestValidator()
        {
            Describe(x => x.Id).IsLengthInclusivelyBetween(4,6);
            Describe(x => x.Name).Vitally().IsLengthInclusivelyBetween(0, 22);
        }
    }
}