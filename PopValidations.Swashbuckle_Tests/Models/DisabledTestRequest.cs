namespace PopValidations.Swashbuckle_Tests.DisabledTests;

public class DisabledTestRequest
{
    public string Id { get; set; }
}

public class DisabledTestRequestValidator : AbstractValidator<DisabledTestRequest>
{
    public DisabledTestRequestValidator()
    {
        Describe(x => x.Id).NotNull();
    }
}