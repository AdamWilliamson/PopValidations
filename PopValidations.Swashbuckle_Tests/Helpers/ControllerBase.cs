using Microsoft.AspNetCore.Mvc;

namespace PopValidations.Swashbuckle_Tests.Helpers;

[ApiController]
[Route("api/[controller]")]
public abstract class ControllerBase<TRequest> : Controller
{
    [HttpGet("test")]
    public object? Get(TRequest request)
    {
        return request;
    }
}