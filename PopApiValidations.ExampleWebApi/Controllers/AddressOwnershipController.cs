using Microsoft.AspNetCore.Mvc;
using PopApiValidations.ExampleWebApi.DataSources;

namespace PopApiValidations.ExampleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class AddressOwnershipController : ControllerBase
{
    private readonly ILogger<AddressOwnershipController> _logger;

    public AddressOwnershipController(ILogger<AddressOwnershipController> logger)
    {
        _logger = logger;
    }

    [HttpGet(nameof(Get))]
    public IEnumerable<AddressOwnership> Get()
    {
        return AddressOwnershipDataSource.Instance.Items.ToList();
    }

    [HttpGet($"{nameof(GetById)}/{{id}}")]
    public AddressOwnership? GetById(int id)
    {
        return AddressOwnershipDataSource.Instance.GetAddressOwnershipById(id);
    }

    [HttpPost(nameof(AddAddressOwnership))]
    public AddressOwnership AddAddressOwnership([FromBody]AddressOwnership newAddressOwnership)
    {
        AddressOwnershipDataSource.Instance.AddRecord(newAddressOwnership);
        return newAddressOwnership;
    }

    [HttpPost(nameof(AddAddressOwnership_Ignore))]
    [PopApiValidationsIgnore]
    public AddressOwnership AddAddressOwnership_Ignore([FromBody] AddressOwnership newAddressOwnership)
    {
        AddressOwnershipDataSource.Instance.AddRecord(newAddressOwnership);
        return newAddressOwnership;
    }

    [HttpPost(nameof(AddAddressOwnership_Rename))]
    public AddressOwnership AddAddressOwnership_Rename(
        [FromBody][PopApiValidationsRenameParam("CreationAddress")] AddressOwnership newAddressOwnership
    )
    {
        AddressOwnershipDataSource.Instance.AddRecord(newAddressOwnership);
        return newAddressOwnership;
    }
        
    [HttpDelete()]
    public IActionResult Delete(int id)
    {
        AddressOwnershipDataSource.Instance.DeleteAddressOwnership(id);
        return Ok();
    }

    [HttpPatch()]
    public IActionResult Update(AddressOwnership patch)
    {
        return Ok();
    }


    [HttpPut($"{{id}}/{nameof(InsertOwner)}")]
    public IActionResult InsertOwner(int id, string? firstName, string? lastName, ContactRecordType? contactType, string? contactValue)
    {
        return Ok();
    }

    [HttpGet(nameof(GetDumbTest))]
    public IActionResult GetDumbTest(DumbTestRequest request)
    {
        return Ok();
    }
}

public class DumbTestRequest
{
    public int Temporal { get; set; }
}