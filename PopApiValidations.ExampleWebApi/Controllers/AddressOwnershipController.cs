using Microsoft.AspNetCore.Mvc;
using PopApiValidations.ExampleWebApi.DataSources;

namespace PopApiValidations.ExampleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressOwnershipController : ControllerBase
{
    private readonly ILogger<AddressOwnershipController> _logger;

    public AddressOwnershipController(ILogger<AddressOwnershipController> logger)
    {
        _logger = logger;
    }

    [HttpGet()]
    public IEnumerable<AddressOwnership> Get()
    {
        return AddressOwnershipDataSource.Instance.Items.ToList();
    }

    [HttpGet("{id}")]
    public AddressOwnership? GetById(int id)
    {
        return AddressOwnershipDataSource.Instance.GetAddressOwnershipById(id);
    }

    [HttpPost()]
    public AddressOwnership AddAddressOwnership(AddressOwnership newAddressOwnership)
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
}
