using PopValidations;
using ApiValidations;
using Microsoft.AspNetCore.Mvc;

namespace PopApiValidations.ExampleWebApi.Controllers;

public class AddressOwnershipControllerValidation : ApiValidator<AddressOwnershipController>
{
    public AddressOwnershipControllerValidation()
    {
        DescribeFunc(x => x.Get()).Return.IsNotNull();

        DescribeFunc(x => x.GetById(Param.Is<int>().IsGreaterThan(0)));

        DescribeFunc(x => x.Delete(Param.Is<int>().IsGreaterThan(0)));

        DescribeFunc(x => x.AddAddressOwnership(Param.Is<AddressOwnership>().SetValidator(new CreatingAddressOwnershipValidation())));

        DescribeFunc(x => x.InsertOwner(
            Param.Is<int>().IsGreaterThan(0),
            Param.Is<string?>().IsNotNull(),
            Param.Is<string?>().IsNotNull(),
            Param.Is<ContactRecordType?>().IsNotNull(),
            Param.Is<string?>().IsNotNull()
        ));
    }
}

public class CreatingAddressOwnershipValidation: ApiSubValidator<AddressOwnership>
{
    public CreatingAddressOwnershipValidation()
    {
        Describe(x => x.Id).IsNull();
        Describe(x => x.Address).SetValidator(new CreatingAddressValidation());
        DescribeEnumerable(x => x.Owners).Vitally().IsNotNull().ForEach(x => x.SetValidator(new CreatingPersonValidation()));
    }
}

public class CreatingAddressValidation : ApiSubValidator<Address>
{
    public CreatingAddressValidation()
    {
        Describe(x => x.Id).IsNull();
    }
}

public class CreatingPersonValidation : ApiSubValidator<Person>
{
    public CreatingPersonValidation()
    {
        Describe(x => x.Id).IsNull();
        DescribeEnumerable(x => x.ContactRecords).Vitally().IsNotNull().ForEach(x => x.SetValidator(new CreatingContactRecordValidation()));
    }
}

public class CreatingContactRecordValidation : ApiSubValidator<ContactRecord>
{
    public CreatingContactRecordValidation()
    {
        Describe(x => x.Id).IsNull();
    }
}