using PopValidations;
using ApiValidations;

namespace PopApiValidations.ExampleWebApi.Controllers;

public class AddressOwnershipControllerValidation : ApiValidator<AddressOwnershipController>
{
    public AddressOwnershipControllerValidation()
    {
        DescribeFunc(x => x.Get()).Return.IsNotNull().ForEach(x => x.IsNotNull());

        DescribeFunc(x => x.GetById(Param.Is<int>().IsGreaterThan(0)));

        DescribeFunc(x => x.Delete(Param.Is<int>().IsGreaterThan(0)));

        DescribeFunc(x => 
            x.AddAddressOwnership(
                Param.Is<AddressOwnership>().IsNotNull().SetValidator(new CreatingAddressOwnershipValidation())
            )
        ).Return.IsNotNull().SetValidator(new CreatingAddressOwnershipValidation());

        DescribeFunc(x => x.AddAddressOwnership_Ignore(
            Param.Is<AddressOwnership>().SetValidator(new CreatingAddressOwnershipValidation()))
        );

        DescribeFunc(x => x.AddAddressOwnership_Rename(
            Param.Is<AddressOwnership>().SetValidator(new CreatingAddressOwnershipValidation()))
        );

        DescribeFunc(x => x.InsertOwner(
            Param.Is<int>().IsGreaterThan(0),
            Param.Is<string?>().IsNotNull(),
            Param.Is<string?>().IsNotNull(),
            Param.Is<ContactRecordType?>().IsNotNull(),
            Param.Is<string?>().IsNotNull()
        ));

        DescribeFunc(x => x.MultipleContentParameters(
            Param.Is<AddressOwnership>().Vitally().IsNotNull().SetValidator(new CreatingAddressOwnershipValidation()),
            Param.Is<int>().IsGreaterThan(0).IsLessThan(9999),
            Param.Is<string>().Vitally().IsNotNull().IsLengthInclusivelyBetween(0, 9999)
        ));

        DescribeFunc(x => x.BasicQueryArray(Param.IsEnumerable<int>().IsNotNull().ForEach(i => i.IsGreaterThan(3)).Convert<List<int>>()));

        DescribeFunc(x => x.ObjectQueryArray(
            Param.IsEnumerable<ContactRecord>()
                .IsNotNull()
                .ForEach(i => i.IsNotNull().SetValidator(new CreatingContactRecordValidation())).Convert<List<ContactRecord>>()
            )
        );

        DescribeFunc(x => x.ObjectBodyArray(
            Param.IsEnumerable<AddressOwnership>()
                .IsNotNull()
                .ForEach(i => i.IsNotNull().SetValidator(new CreatingAddressOwnershipValidation())).Convert<List<AddressOwnership>>()
            )
        );
    }
}

public class CreatingAddressOwnershipValidation: ApiSubValidator<AddressOwnership>
{
    public CreatingAddressOwnershipValidation()
    {
        Describe(x => x.Id).IsNull();
        Describe(x => x.Address).Vitally().IsNotNull().SetValidator(new CreatingAddressValidation());
        DescribeEnumerable(x => x.Owners).Vitally().IsNotNull().ForEach(x => x.IsNotNull().SetValidator(new CreatingPersonValidation()));
        DescribeEnumerable(x => x.InteractionRating).Vitally().IsNotNull().ForEach(x => x.IsNotNull());
    }
}

public class CreatingAddressValidation : ApiSubValidator<Address>
{
    public CreatingAddressValidation()
    {
        Describe(x => x.Id).IsNull();
        Describe(x => x.StreetNumber).IsGreaterThan(0).IsLessThan(999999);
        Describe(x => x.StreetName).Vitally().IsNotEmpty().IsLengthInclusivelyBetween(3, 200);
        Describe(x => x.Suburb).Vitally().IsNotEmpty().IsLengthInclusivelyBetween(3, 200);
        Describe(x => x.Postcode).Vitally().IsGreaterThan(0).IsLessThan(999999);
    }
}

public class CreatingPersonValidation : ApiSubValidator<Person>
{
    public CreatingPersonValidation()
    {
        Describe(x => x.Id).IsNull();
        Describe(x => x.Age).IsGreaterThan(0).IsLessThan(120);
        Describe(x => x.FirstName).Vitally().IsNotEmpty().IsLengthInclusivelyBetween(2, 120);
        Describe(x => x.LastName).Vitally().IsNotEmpty().IsLengthInclusivelyBetween(2, 120);

        DescribeEnumerable(x => x.ContactRecords).Vitally().IsNotNull().ForEach(x => x.SetValidator(new CreatingContactRecordValidation()));
    }
}

public class CreatingContactRecordValidation : ApiSubValidator<ContactRecord>
{
    public CreatingContactRecordValidation()
    {
        Describe(x => x.Id).IsNull();
        Describe(x => x.Value).IsNotNull();

        When(
            "Type is email", 
            x => Task.FromResult(x.Type == ContactRecordType.Email), 
            () =>
            {
                Describe(x => x.Value).IsEmail();
            });

        When(
            "Type is Mobile Or Home Number",
            x => Task.FromResult(x.Type == ContactRecordType.Mobile || x.Type == ContactRecordType.HomeNumber),
            () =>
            {
                Describe(x => x.Value).IsNotEmpty();
            });
    }
}