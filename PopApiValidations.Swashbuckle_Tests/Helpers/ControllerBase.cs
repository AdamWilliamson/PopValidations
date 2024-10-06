using ApiValidations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PopApiValidations.Swashbuckle_Tests.Helpers;

public class RequestDataItem
{
    public string Identifier { get; set; }
    public object Value { get; set; }
}

public abstract class AbstractComplexObject
{
    [Required]
    public int IntegerField { get; set; }
    [Required]
    public string StringField { get; set; }
    public List<string> ListOfStringsField { get; set; }
    public RequestDataItem DataItemField { get; set; }
    public List<RequestDataItem> ListOfRequestDataItemsField { get; set; }
    public Dictionary<string, int> DictOfStringIntField { get; set; }
}

public class SubRequest : AbstractComplexObject
{ 
}

public class Request : AbstractComplexObject
{
    public SubRequest SubRequestField { get; set; }
}

public class TestControllerValidation : ApiValidator<TestController> { }

[ApiController]
[Route("api/[controller]")]
public class TestController : Controller
{
    [HttpGet()]
    public ActionResult<List<Response>> GetAll()
    {
        return new List<Response>();
    }

    [HttpGet("{id}")]
    public ActionResult<Response> GetById(int? id)
    {
        return Ok();
    }

    [HttpPost()]
    public ActionResult<Response> Create(Request request)
    {
        return Ok();
    }

    [HttpPost(nameof(CreateByUrl) +"/{id}/{stringField}/{listOfIntField}")]
    public ActionResult<Response> CreateByUrl(int id, string stringField, List<int> listOfIntField)
    {
        return Ok();
    }

    [HttpPost(nameof(CreateByQuery))]
    public ActionResult<Response> CreateByQuery([FromQuery]Request request)
    {
        return Ok();
    }

    [HttpPost(nameof(CreateByBody))]
    public ActionResult<Response> CreateByBody([FromBody] Request request)
    {
        return Ok();
    }

    [HttpPut()]
    public ActionResult<Response> Update(Request request)
    {
        return Ok();
    }

    [HttpPost(nameof(UpdateByUrl) + "/{id}/{stringField}/{listOfIntField}")]
    public ActionResult<Response> UpdateByUrl(int id, string stringField, List<int> listOfIntField)
    {
        return Ok();
    }

    [HttpPut(nameof(UpdateByQuery))]
    public ActionResult<Response> UpdateByQuery([FromQuery]Request request)
    {
        return Ok();
    }

    [HttpPut(nameof(UpdateByBody))]
    public ActionResult<Response> UpdateByBody([FromBody]Request request)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        return Ok();
    }

    [HttpDelete(nameof(DeleteByQuery))]
    public IActionResult DeleteByQuery([FromQuery]int id)
    {
        return Ok();
    }

    [HttpDelete(nameof(DeleteByBody))]
    public IActionResult DeleteByBody([FromBody] int id)
    {
        return Ok();
    }
}

public class SubResponse : AbstractComplexObject { }
public class Response : AbstractComplexObject 
{
    public SubResponse SubResponseField { get; set; }
    public List<SubResponse> ListOfSubResponseField { get; set; }
}