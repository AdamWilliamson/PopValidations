using ApiValidations;
using Microsoft.AspNetCore.Mvc;

namespace PopApiValidations.Swashbuckle_Tests.ValidationModificationTests.TestClasses;

[ApiController]
[Route("api/GenericVariantsController")]
public class GenericVariantsController<TType> : Controller
{
    [HttpGet]
    public TType Get(TType value) { return value; }

    [HttpPost]
    public TType Post(TType value) { return value; }

    [HttpPut]
    public TType Put(TType value) { return value; }

    [HttpDelete]
    public TType Delete(TType value) { return value; }

    [HttpPatch]
    public TType Patch(TType value) { return value; }

    [HttpHead]
    public TType Head(TType value) { return value; }

    [HttpOptions]
    public TType Options(TType value) { return value; }
}

public class IntVariantsControllerValidator<TType> : ApiValidator<GenericVariantsController<TType>>
{
    public IntVariantsControllerValidator() 
    {
        DescribeFunc(x => x.Get(Param.Is<TType>().IsNotNull())).Return.IsNotNull();

        DescribeFunc(x => x.Post(Param.Is<TType>().IsNotNull())).Return.IsNotNull();

        DescribeFunc(x => x.Put(Param.Is<TType>().IsNotNull())).Return.IsNotNull();

        DescribeFunc(x => x.Delete(Param.Is<TType>().IsNotNull())).Return.IsNotNull();

        DescribeFunc(x => x.Patch(Param.Is<TType>().IsNotNull())).Return.IsNotNull();

        DescribeFunc(x => x.Head(Param.Is<TType>().IsNotNull())).Return.IsNotNull();

        DescribeFunc(x => x.Options(Param.Is<TType>().IsNotNull())).Return.IsNotNull();
    }
}