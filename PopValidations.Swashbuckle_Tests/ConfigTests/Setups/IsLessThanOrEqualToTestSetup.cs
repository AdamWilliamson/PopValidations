﻿using PopValidations.Swashbuckle_Tests.Helpers;

namespace PopValidations.Swashbuckle_Tests.ConfigTests.Setups;

public static class IsLessThanOrEqualToTestSetup
{
    public class TestController : ControllerBase<Request> { }

    public record Request(int? Id, string? Name, SubRequest Child);
    public record SubRequest(int? Id, string? Name);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            Describe(x => x.Id).IsLessThanOrEqualTo(5);
            Describe(x => x.Name).Vitally().IsLessThanOrEqualTo("Test");
            Describe(x => x.Child).SetValidator(new SubRequestValidator());
        }
    }

    public class SubRequestValidator : AbstractSubValidator<SubRequest>
    {
        public SubRequestValidator()
        {
            Describe(x => x.Id).IsLessThanOrEqualTo(7);
            Describe(x => x.Name).Vitally().IsLessThanOrEqualTo("Test2");
        }
    }
}