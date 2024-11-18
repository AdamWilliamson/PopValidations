using ApiValidations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PopApiValidations.Swashbuckle_Tests.Internal;

public class SubRequest
{
    [JsonProperty(PropertyName = "TooDeepInteger")]
    public int Integer { get; set; }

    public List<int> Integers { get; set; } = new();
}

public class Request
{
    public int Integer { get; set; }

    [JsonProperty(PropertyName = "NewName")]
    public string Renamed { get; set; }

    public SubRequest SubRequest { get; set; }

    [JsonProperty(PropertyName = "RenamedSubRequest")]
    public SubRequest SubRequest2 { get; set; }

    public List<SubRequest> SubRequests { get; set; }
}

[ApiController]
public class Test_ApiController : Controller
{
    public void Function1(Request request) { }

    [HttpGet(nameof(QueryFunction))]
    public void QueryFunction([FromQuery] Request request) { }
    [HttpGet(nameof(QueryListFunction))]
    public void QueryListFunction([FromQuery] List<Request> requests) { }
    [HttpGet(nameof(NamedQueryFunction))]
    public void NamedQueryFunction([FromQuery(Name = "NewRequest")] Request request) { }

    [HttpPost]
    public void PostFunction([FromBody] Request request) { }
    [HttpPost]
    public void FormFunction([FromForm] Request request) { }
    [HttpPost]
    public void NamedFormFunction([FromForm(Name = "NewFormName")] Request request) { }

    [HttpPut(nameof(BasicRouteFunction))]
    public void BasicRouteFunction([FromRoute] int id) { }
    [HttpPut(nameof(NamedBasicRouteFunction))]
    public void NamedBasicRouteFunction([FromRoute(Name = "NewId")] int id) { }
    [HttpPut(nameof(ObjectRouteFunction))]
    public void ObjectRouteFunction([FromRoute(Name = "NewUrlRequest")] Request request) { }

    [HttpGet(nameof(HeaderFunction))]
    public void HeaderFunction([FromQuery] Request request) { }
    [HttpGet(nameof(NamedHeaderFunction))]
    public void NamedHeaderFunction([FromQuery(Name = "NewHeaderName")] Request request) { }
    [HttpGet(nameof(RouteQueryAndBodyFunction))]
    public void RouteQueryAndBodyFunction([FromRoute] int id, [FromQuery(Name = "NewHeaderName")] Request request, [FromBody] Request body) { }
}

public class Test_ApiValidation : ApiValidator<Test_ApiController> { }
